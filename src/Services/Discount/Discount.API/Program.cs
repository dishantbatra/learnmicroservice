using Common.Logging;
using Discount.API.Extensions;
using Discount.API.Repositories;
using Discount.API.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Host.UseSerilog(SeriLogger.Configure);
var app = builder.Build();
app.UseSerilogRequestLogging();
app.MigrateDatabase<Program>(5);
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));

}

app.MapControllers();
app.Run();
