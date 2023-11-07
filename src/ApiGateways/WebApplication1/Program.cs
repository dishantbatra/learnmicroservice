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

builder.Services.AddOcelot();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseOcelot();

app.Run();
