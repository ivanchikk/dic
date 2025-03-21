using FruitsBasket.Api.Fruit;
using FruitsBasket.Data;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Fruit;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();

        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddScoped<IFruitRepository, FruitRepository>();
        services.AddScoped<IFruitOrchestrator, FruitOrchestrator>();

        services.AddAutoMapper(typeof(FruitProfile), typeof(FruitDaoProfile));
        ConfigureDb(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    protected virtual void ConfigureDb(IServiceCollection services)
    {
        services.AddDbContext<FruitDbContext>(
            (sp, options) => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
        );
    }
}