using DistributedBanking.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddApi()
    .AddSwagger()
    .AddServices(configuration)
    .ConfigureOptions(configuration);

builder.Host.UseSerilogAppLogging();

var application = builder.Build();
application
    .UseAppSerilog()
    .UseAppCore()
    .UseAppSwagger();

await application.RunAsync();
