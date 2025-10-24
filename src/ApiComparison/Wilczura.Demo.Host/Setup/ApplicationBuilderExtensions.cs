using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using Npgsql;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Wilczura.Demo.Common;
using Wilczura.Demo.Common.Activities;
using Wilczura.Demo.Common.Consts;
using Wilczura.Demo.Common.Exceptions;
using Wilczura.Demo.Common.Logging;
using Wilczura.Demo.Common.Models;
using Wilczura.Demo.Persistence.Models;
using Wilczura.Demo.Persistence.Repositories;

namespace Wilczura.Demo.Host.Setup;

public static class ApplicationBuilderExtensions
{
    private const string ServiceOptionsKey = "ServiceOptions";

    public static ILogger GetStartupLogger(this IHostApplicationBuilder app, string domainName)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            builder.AddSimpleConsole(config =>
            {
                config.IncludeScopes = false;
                config.SingleLine = true;
            });
        });

        var logger = loggerFactory.CreateLogger<ServiceOptions>();

        if (logger == null)
        {
            throw new CommonException(nameof(logger));
        }

        logger.LogInformation("Logger created");
        logger.LogInformation("{systemInfo}", SystemInfo.GetInfo(domainName));

        return logger;
    }


    public static WebApplicationBuilder AddCommonServices(this WebApplicationBuilder builder, string domainName, ILogger logger)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        _ = new CustomActivityListener(CustomActivitySource.Name, logger);

        var serviceOptions = GetServiceOptions(builder);

        SetupObservability(builder, logger);

        return builder;
    }

    public static WebApplicationBuilder AddOtherServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ODataRepository<Country>, CountryODataRepository>();
        return builder;
    }

    public static IMvcBuilder AddWebServices(
        this WebApplicationBuilder builder,
        AssemblyPart controllersAssemblyPart,
        ILogger? logger = null)
    {
        var mvcBuilder = builder.Services.AddControllers(o =>
        {
            o.Conventions.Add(new CustomConvention());
        })
        .ConfigureApplicationPartManager(setupAction =>
        {
            setupAction.ApplicationParts.Clear();
            setupAction.ApplicationParts.Add(controllersAssemblyPart);
        })
        .AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/docs/configure-and-customize-swaggergen.md#generate-multiple-swagger-documents
            options.SwaggerDoc("odata", new OpenApiInfo { Title = "OData API", Version = "v1" });
            options.SwaggerDoc("custom", new OpenApiInfo { Title = "Custom API", Version = "v1" });
        });

        var serviceOptions = GetServiceOptions(builder);

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CommonConsts.CorsPolicyName, policy =>
            {
                policy.WithOrigins(serviceOptions.Cors ?? [])
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return mvcBuilder;
    }

    private static ServiceOptions GetServiceOptions(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.GetSection(ServiceOptionsKey);
        builder.Services.Configure<ServiceOptions>(config);

        var serviceOptions = new ServiceOptions();
        config.Bind(serviceOptions);

        return serviceOptions;
    }

    public static IHostApplicationBuilder AddPostgres<TContext>(
        this IHostApplicationBuilder app,
        string connectionStringName,
        string? sectionName,
        ILogger? logger) where TContext : DbContext
    {
        // connectionStringName should be the same as domain name.
        // schema is lowercase (for convenience)
        // so if we have domain "Demo" then connection string should be named "Demo"
        // this will result in database sechema to be named "demo"
        app.Services.AddDbContextPool<TContext>((serviceProvider, opt) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            IConfiguration section = string.IsNullOrWhiteSpace(sectionName)
            ? configuration
            : configuration.GetSection(sectionName);
            var connectionString = section.GetConnectionString(connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = section.GetConnectionString(connectionStringName.ToLowerInvariant());
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                logger?.LogError(LogInfo.From($"Connection string is empty").WithAdditionalInfo($"{connectionStringName}"));
            }
            else
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                var schemaName = ((dataSourceBuilder.ConnectionStringBuilder.SearchPath?.Split(',') ?? [])
                    .FirstOrDefault() ?? connectionStringName).ToLowerInvariant();
                var dataSource = dataSourceBuilder.Build();

                opt.UseNpgsql(dataSource, x => x.MigrationsHistoryTable("__ef_migrations_history", schemaName))
                    .UseSnakeCaseNamingConvention();
            }
        });
        return app;
    }


    private static void SetupObservability(this WebApplicationBuilder builder, ILogger logger)
    {
        //TODO: identify logging for sql queries generated by entity framework
        logger?.LogInformation("Disabling default log providers. Enabling NLog.");
        builder.Logging.ClearProviders();

        var layout = new JsonLayout()
        {
            IncludeEventProperties = true,
            IncludeScopeProperties = true,
            MaxRecursionLimit = 3,
        };
        layout.Attributes.Add(new NLog.Layouts.JsonAttribute("Time", Layout.FromString("${longdate}")));
        layout.Attributes.Add(new NLog.Layouts.JsonAttribute("Level", Layout.FromString("${level:upperCase=true}")));
        layout.Attributes.Add(new NLog.Layouts.JsonAttribute("Logger", Layout.FromString("${logger}")));
        layout.Attributes.Add(new NLog.Layouts.JsonAttribute("ActivityId", Layout.FromString("${activityid}")));
        // layout.Attributes.Add(new NLog.Layouts.JsonAttribute("Message", Layout.FromString("${message}")));

        var nlogConfiguration = new LoggingConfiguration();
        var consoleTarget = new ConsoleTarget()
        {
            Layout = layout
        };
        nlogConfiguration.AddTarget("console", consoleTarget);

        nlogConfiguration.LoggingRules.Add(new LoggingRule("Wilczura.*", NLog.LogLevel.Debug, consoleTarget)
        {
            Final = false
        });

        nlogConfiguration.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, consoleTarget));

        builder.Logging.AddNLog(nlogConfiguration);
    }

}
