using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging((hostingContext, loggingBuilder) =>
{
    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    // When you call logging.ClearProviders(), you are essentially clearing any existing logging providers that might have been added
    // by default or in previous configurations. 
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    // If you don't add the AddDebug() logging provider, your application won't log messages to the debug output.
    // The absence of AddDebug() simply means that your application will not utilize the debug output as one of its logging destinations.
    loggingBuilder.AddDebug();
});

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    //Based on environment file will be picked.
    // second parameter we use to be true  as it specifies that if the file is not found, the application will continue to run without throwing an error.
    config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true); 
});

builder.Services.
    AddOcelot()
    .AddCacheManager(x=>x.WithDictionaryHandle());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseOcelot();

app.Run();
