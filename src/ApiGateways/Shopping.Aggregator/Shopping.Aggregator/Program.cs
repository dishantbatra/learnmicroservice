using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddControllers();
//These all are transient in dependency injection

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c=>c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]));
builder.Services.AddHttpClient<IBasketService, BasketService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]));
builder.Services.AddHttpClient<IOrderService, OrderService>(c => c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();
app.Run();