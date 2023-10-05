using System.Reflection;
using DistributedBanking.Data.Repositories;
using DistributedBanking.Data.Repositories.Implementation;
using DistributedBanking.Data.Services;
using DistributedBanking.Data.Services.Implementation;
using DistributedBanking.Domain.Options;
using DistributedBanking.Domain.Services;
using DistributedBanking.Domain.Services.Implementation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace DistributedBanking.Extensions;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        
        services
            .AddRouting()
            .AddEndpointsApiExplorer()
            .AddHttpContextAccessor();
        
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
            });
        
        return services;
    }
    
    internal static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        
        return services;
    }
    
    internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMongoDatabase(configuration)
            .AddDataRepositories();

        services.AddTransient<IAccountService, AccountService>();
        
        return services;
    }
    
    private static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        ArgumentNullException.ThrowIfNull(databaseOptions);
        
        services.AddSingleton<IMongoDbFactory>(new MongoDbFactory(databaseOptions.ConnectionString, databaseOptions.DatabaseName));
        
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };
        ConventionRegistry.Register("CamelCaseElementNameConvention", pack, _ => true);
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(DateTimeKind.Utc));
        
        return services;
    }
    
    private static IServiceCollection AddDataRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddTransient<IAccountsRepository, AccountsRepository>();

        return services;
    }
}