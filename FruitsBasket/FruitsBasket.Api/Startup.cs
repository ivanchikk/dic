using Asp.Versioning;
using FruitsBasket.Api.Fruit;
using FruitsBasket.Data;
using FruitsBasket.Data.Basket;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Infrastructure.BlobStorage;
using FruitsBasket.Model.Basket;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Model.FruitBasket;
using FruitsBasket.Orchestrator.Basket;
using FruitsBasket.Orchestrator.Fruit;
using FruitsBasket.Orchestrator.FruitBasket;
using Microsoft.EntityFrameworkCore;

namespace FruitsBasket.Api;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader(),
                new HeaderApiVersionReader(),
                new MediaTypeApiVersionReader()
            );
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddSwaggerGen();

        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddScoped<IFruitRepository, FruitRepository>();
        services.AddScoped<IFruitOrchestrator, FruitOrchestrator>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketOrchestrator, BasketOrchestrator>();
        services.AddSingleton<IBlobStorage, BlobStorage>();
        services.AddScoped<IFruitBasketOrchestrator, FruitBasketOrchestrator>();

        services.AddAutoMapper(typeof(FruitProfile), typeof(FruitDaoProfile));
        ConfigureDb(services);
        ConfigureBlobStorage(services);
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
        services.AddDbContext<SqlDbContext>(
            (sp, options) => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
        );
        services.AddDbContext<CosmosDbContext>(
            (sp, options) => options
                .UseCosmos(configuration.GetConnectionString("CosmosConnection")!, "fruits-basket")
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
        );
    }

    protected virtual void ConfigureBlobStorage(IServiceCollection services)
    {
        var blobConfig = new BlobConfiguration();
        configuration.Bind("AzureBlobStorage", blobConfig);
        services.AddSingleton(blobConfig);
    }
}