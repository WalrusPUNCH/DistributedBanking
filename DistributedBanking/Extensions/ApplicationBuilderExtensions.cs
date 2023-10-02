using Serilog;

namespace DistributedBanking.Extensions;

public static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseAppCore(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder
            .UseRouting()
            .UseEndpoints(conf =>
            {
                conf.MapControllers();
            })
            .UseCookiePolicy();

        return applicationBuilder;
    }
    
    internal static IApplicationBuilder UseAppSerilog(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseSerilogRequestLogging();

        return applicationBuilder;
    }
    
    internal static IApplicationBuilder UseAppSwagger(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder
            .UseSwagger()
            .UseSwaggerUI(options => 
            { 
                options.RoutePrefix = string.Empty;
                
                options.ShowCommonExtensions(); 
                options.ShowExtensions(); 
                options.DisplayRequestDuration(); 
                options.DisplayOperationId(); 
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

        return applicationBuilder;
    }
}