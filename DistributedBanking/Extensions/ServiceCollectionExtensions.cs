﻿using System.Globalization;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Data.Services;
using DistributedBanking.Data.Services.Implementation;
using DistributedBanking.Domain.Options;
using DistributedBanking.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DistributedBanking.Data.Models;
using DistributedBanking.Data.Models.Identity.Default;
using DistributedBanking.Data.Repositories.Implementation.Default;
using DistributedBanking.Data.Repositories.Implementation.Default.Base;
using DistributedBanking.Data.Repositories.Implementation.TransactionalClock;
using DistributedBanking.Domain.Models.Transaction;
using DistributedBanking.Domain.Services.Base;
using DistributedBanking.Domain.Services.Base.Implementation;
using DistributedBanking.Domain.Services.Default;
using DistributedBanking.Domain.Services.Default.Implementation;
using DistributedBanking.Domain.Services.TransactionalClock;
using DistributedBanking.Domain.Services.TransactionalClock.Implementation;
using Mapster;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Converters;
using Refit;
using TransactionalClock.Integration;
using TransactionalClock.Integration.DelegationHandlers;

namespace DistributedBanking.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        ArgumentNullException.ThrowIfNull(jwtOptions);
        
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services
            .AddRouting()
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });
        
        services.AddAuthorization();
        
        services
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor();
        
        services.AddMapster();
        
        return services;
    }
    
    internal static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        
        services
            .AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                options.UseInlineDefinitionsForEnums();
                
                // JWT
                options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the Bearer Authorization string as following: `Bearer *JWT-Token*`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        
        return services;
    }
    
    internal static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = CustomInvalidModelStateResponseFactory.MakeFailedValidationResponse;
        });

        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        
        return services;
    }
    
    internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var transactionalClockOptions = configuration.GetSection(nameof(TransactionalClockOptions)).Get<TransactionalClockOptions>();
        ArgumentNullException.ThrowIfNull(transactionalClockOptions);

        services
            .AddMongoDatabase(configuration)
            .AddDataRepositories(configuration);
        
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<ITransactionService, TransactionService>();
        
        if (transactionalClockOptions.UseTransactionalClock)
        {
            services
                .AddTransactionalClockIntegration(configuration)
                .AddTransient<IPasswordHashingService, PasswordHashingService>()
                .AddTransient<ITcRolesManager, TcRolesManager>()
                .AddTransient<ITcUserManager, TcUserManager>()
                .AddTransient<IIdentityService, IdentityTcService>()
                .AddTransient<ITcTokenService, TcTokenService>();
        }
        else
        {
            services.AddMongoIdentity(configuration);

            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IIdentityService, IdentityService>();
        }
        
        return services;
    }

    private static IServiceCollection AddMapster(this IServiceCollection services)
    {
        /*TypeAdapterConfig<Person, PersonShortInfoDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => $"{src.Title} {src.FirstName} {src.LastName}");*/

        TypeAdapterConfig<TransactionEntity, TransactionResponseModel>.NewConfig()
            .Map(dest => dest.Timestamp, src => src.DateTime);

        TypeAdapterConfig<ObjectId, string>.NewConfig()
            .MapWith(objectId => objectId.ToString());

        TypeAdapterConfig<string, ObjectId>.NewConfig()
            .MapWith(value => new ObjectId(value));
                
        
        return services;
    }
    
    private static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var transactionalClockOptions = configuration.GetSection(nameof(TransactionalClockOptions)).Get<TransactionalClockOptions>();
        ArgumentNullException.ThrowIfNull(transactionalClockOptions);
        
        var databaseOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        ArgumentNullException.ThrowIfNull(databaseOptions);

        services.AddSingleton<IMongoDbFactory>(transactionalClockOptions.UseTransactionalClock
            ? new MongoDbFactory(databaseOptions.ConnectionString, databaseOptions.DatabaseName)
            : new MongoDbFactory(databaseOptions.ReplicaSetConnectionString, databaseOptions.DatabaseName));

        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreIfNullConvention(true)
        };
        ConventionRegistry.Register("CamelCase_StringEnum_IgnoreNull_Convention", pack, _ => true);
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));

       return services;
    }

    private static IServiceCollection AddMongoIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        ArgumentNullException.ThrowIfNull(databaseOptions);
        
        services.AddIdentity<ApplicationUser, ApplicationRole>(identityOptions => 
            { 
                // Password settings
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireLowercase = true; 
                identityOptions.Password.RequireUppercase = true; 
                identityOptions.Password.RequireNonAlphanumeric = false; 
                identityOptions.Password.RequireDigit = true;

                // Lockout settings
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                identityOptions.Lockout.MaxFailedAccessAttempts = 10;

                // User settings
                identityOptions.User.RequireUniqueEmail = true;
                identityOptions.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            })
            .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(
                databaseOptions.ReplicaSetConnectionString, databaseOptions.DatabaseName);
        
        return services;
    }
    
    private static IServiceCollection AddDataRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        var transactionalClockOptions = configuration.GetSection(nameof(TransactionalClockOptions)).Get<TransactionalClockOptions>();
        ArgumentNullException.ThrowIfNull(transactionalClockOptions);
        
        if (transactionalClockOptions.UseTransactionalClock)
        {
            services.AddTransient<IUsersTcRepository, UsersTcRepository>();
            services.AddTransient<IRolesTcRepository, RolesTcRepository>();
            services.AddTransient<IAccountsRepository, AccountsTcRepository>();
            services.AddTransient<ICustomersRepository, CustomersTcRepository>();
            services.AddTransient<IWorkersRepository, WorkersTcRepository>();
            services.AddTransient<ITransactionsRepository, TransactionsTcRepository>();
        }
        else
        { 
            services.AddTransient<IAccountsRepository, AccountsRepository>();
            services.AddTransient<ICustomersRepository, CustomersRepository>();
            services.AddTransient<IWorkersRepository, WorkersRepository>();
            services.AddTransient<ITransactionsRepository, TransactionsRepository>();
        }
        
        return services;
    }

    private static IServiceCollection AddTransactionalClockIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        var transactionalClockOptions = configuration.GetSection(nameof(TransactionalClockOptions)).Get<TransactionalClockOptions>();
        ArgumentNullException.ThrowIfNull(transactionalClockOptions);

        IsoDateTimeConverter converter = new IsoDateTimeConverter
        {
            DateTimeStyles = DateTimeStyles.AdjustToUniversal,
            DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK"
        };
        
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                Converters = { new ObjectIdJsonConverter() }
            })
        };

        var httpClientBuilder = services
            .AddRefitClient<ITransactionalClockClient>(refitSettings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(transactionalClockOptions.TransactionalClockHostUrl));
        
#if DEBUG
        services.TryAddScoped<HttpDebugLoggingHandler>();
        httpClientBuilder.AddHttpMessageHandler<HttpDebugLoggingHandler>();
#endif
        
        return services;
    }
}