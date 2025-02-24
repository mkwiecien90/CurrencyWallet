#region Usings

using System.Reflection;
using CurrencyWallet.Api.Middlewares;
using CurrencyWallet.Application.Behaviors;
using CurrencyWallet.Application.Cache;
using CurrencyWallet.Application.Interfaces;
using CurrencyWallet.Domain.Common;
using CurrencyWallet.Infrastructure.Persistance;
using CurrencyWallet.Infrastructure.Persistance.Repository;
using CurrencyWallet.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

public class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddCommandLine(args)
        .Build();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Assembly.GetExecutingAssembly(),
            Assembly.Load("CurrencyWallet.Application")
        ));

        builder.Services.AddValidatorsFromAssembly(Assembly.Load("CurrencyWallet.Application"));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddAutoMapper(Assembly.Load("CurrencyWallet.Application"));

        Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });

        builder.Services.AddHttpClient(ConstConfig.NbpApi, s =>
        {
            s.BaseAddress = new Uri(ConstConfig.NbpApiUrl);
        });

        builder.Services.AddSingleton<ICurrencyRatesService, CurrencyRatesService>();
        builder.Services.AddHostedService(provider =>
            (CurrencyRatesService)provider.GetRequiredService<ICurrencyRatesService>());
        builder.Services.AddScoped<IWalletRepository, WalletRepository>();
        builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICurrencyCache, CurrencyCache>();

        var app = builder.Build();

        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Initialize database
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (!context.Database.CanConnect())
            {
                context.Database.Migrate();
            }
        }

        await app.RunAsync();
    }
}
