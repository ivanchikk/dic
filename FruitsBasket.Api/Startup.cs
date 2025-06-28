using Asp.Versioning;
using FruitsBasket.Api.Fruit;
using FruitsBasket.Data;
using FruitsBasket.Data.Fruit;
using FruitsBasket.Infrastructure.Email;
using FruitsBasket.Infrastructure.Metrics;
using FruitsBasket.Infrastructure.RabbitMQ;
using FruitsBasket.Model.Fruit;
using FruitsBasket.Orchestrator.Fruit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;

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

        services.AddHostedService<FruitMetricsUpdater>();

        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddScoped<IFruitRepository, FruitRepository>();
        services.AddScoped<IFruitOrchestrator, FruitOrchestrator>();

        services.AddAutoMapper(typeof(FruitProfile), typeof(FruitDaoProfile));

        ConfigureDb(services);
        ConfigureRabbitMQ(services);
        ConfigureEmail(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }

    protected virtual void ConfigureDb(IServiceCollection services)
    {
        services.AddDbContext<SqlDbContext>(
            (sp, options) => options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
        );
    }

    protected virtual void ConfigureRabbitMQ(IServiceCollection services)
    {
        services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IMessageProducer, RabbitMqProducer>();
        services.AddHostedService<FruitEventConsumer>();
    }

    protected virtual void ConfigureEmail(IServiceCollection services)
    {
        services.Configure<EmailConfiguration>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();
    }
}