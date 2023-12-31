using Common.Logging;
using Discount.Grpc.Extensions;
using Discount.Grpc.Repositories;
using Discount.Grpc.Services;
using HealthChecks.UI.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetValue<string>("DatabaseSettings:ConnectionString"));


builder.Host.UseSerilog(SeriLogger.Configure);
var app = builder.Build();
app.UseSerilogRequestLogging();
app.MigrateDatabase<Program>();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapHealthChecks("/hc",new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
