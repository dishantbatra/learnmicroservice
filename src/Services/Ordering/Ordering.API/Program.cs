using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));

// MassTransit-RabbitMQ Configuration
//This line adds MassTransit to the ASP.NET Core service collection. The config parameter is a callback that allows you to configure MassTransit.
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>(); //Adds the BasketCheckoutConsumer to the MassTransit bus.

    config.UsingRabbitMq((ctx, cfg) => { //Configures MassTransit to use RabbitMQ as the transport layer
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]); //Specifies the RabbitMQ host address to connect to.

        //Creates a receive endpoint for the BasketCheckoutQueue and configures it to use the BasketCheckoutConsumer.
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
        });
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase<OrderContext>((context, service) =>
{
    var logger = service.GetService<ILogger<OrderContextSeed>>();
    OrderContextSeed.SeedAsync(context,logger).Wait();
});

app.Run();
