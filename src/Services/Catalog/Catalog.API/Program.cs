using Catalog.API.Data;
using Catalog.API.Repositories;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
}
);
// To register service related to controller

//The AddControllers() method can be extended to customize JSON handling.
//The default for ASP.NET Core is to camel case JSON (first letter small, each subsequent word character capitalized like “carRepo”).
//This matches most of the non-Microsoft frameworks used for web development. However, prior versions of ASP.NET used Pascal casing
//(first letter small, each subsequent word character capitalized like “CarRepo”). The change to camel casing was a breaking change for many applications that were expecting Pascal casing.
//There are two serialization properties that can be set that help with this issue.
//The first is to make the JSON Serializer use Pascal casing by setting its PropertyNamingPolicy to null instead of JsonNamingPolicy.CamelCase.
//The second change is to use case insensitive property names. When this option is enabled, JSON coming into the app can be Pascal or camel cased.
//To make these changes, call AddJsonOptions() on the AddControllers() method:
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.WriteIndented = true; // Not Required
});

builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Host.UseSerilog(SeriLogger.Configure);
builder.Services
.AddHealthChecks()
    .AddMongoDb(builder.Configuration["DatabaseSettings:ConnectionString"], builder.Configuration.GetValue<string>("DatabaseSettings:DatabaseName"), "Catalog MongoDb Health", Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));

}


app.MapControllers();
app.MapHealthChecks("/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();