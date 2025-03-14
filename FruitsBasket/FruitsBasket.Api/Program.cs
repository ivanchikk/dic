using FruitsBasket.Api.Fruit;
using FruitsBasket.Data;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Fruit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SoftDeleteInterceptor>();
builder.Services.AddScoped<IFruitRepository, FruitRepository>();
builder.Services.AddScoped<IFruitOrchestrator, FruitOrchestrator>();
builder.Services.AddAutoMapper(typeof(FruitProfile), typeof(FruitDaoProfile));
builder.Services.AddDbContext<FruitDbContext>(
    (sp, options) => options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseSwagger();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();