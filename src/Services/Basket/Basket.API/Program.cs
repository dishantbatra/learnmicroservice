using System.Reflection;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Common.Logging;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.   
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddGrpcClient<Discount.Grpc.Discount.DiscountClient>(o => o.Address = new Uri(builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl")));
builder.Services.AddScoped<DiscountGrpcService>();

// Redis Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(bus =>
{
    bus.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        //If you don't have receive endpoints you can just set AutoStart = true on the bus configuration.
        //I would suggest that before creating an arbitrary endpoint.
        // https://github.com/MassTransit/MassTransit/issues/2159#issuecomment-742470142

        //what is the impact of setting AutoStart = true if we don't have any active receive endpoint ?
        // Depends on the broker, as it creates the temporary bus queue used for the request client. On RabbitMQ it's almost zero impact,
        // other brokers it may be greater
        // such as SQS since temporary queues are only properly deleted when the process exits with the bus being stopped properly.
        cfg.AutoStart = true; 

    });

    bus.ConfigureHealthCheckOptions(opts =>
    {
        opts.Name = "Rabbit Mq";
        opts.FailureStatus = HealthStatus.Unhealthy;
    });;
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["CacheSettings:ConnectionString"], "Redis Health", Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();

app.MapControllers();
app.MapHealthChecks("/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
