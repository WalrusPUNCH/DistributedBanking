using System.Reflection;

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
    
    internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
     
        return services;
    }
    
    internal static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
       
        return services;
    }
}