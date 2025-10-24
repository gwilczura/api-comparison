using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Wilczura.Demo.Common.Consts;
using Wilczura.Demo.Common.Logging;
using Wilczura.Demo.Common.Models;
using Wilczura.Demo.Host.Middleware;

namespace Wilczura.Demo.Host.Setup;

public static class ApplicationExtensions
{
    private const string ServiceOptionsKey = "ServiceOptions";

    public static async Task ApplyDatabaseMigrationsAsync<TContext>(this WebApplication host)
        where TContext : DbContext
    {
        var config = host.Configuration.GetSection(ServiceOptionsKey);
        var options = new ServiceOptions();
        config.Bind(options);
        await using var scope = host.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var environment = services.GetRequiredService<IHostEnvironment>();

        var logger = services.GetRequiredService<ILogger<TContext>>();

        if (!environment.IsDevelopment() || !options.EnableAutomaticMigration)
        {
            // migrate only in Development and only if explicitely enabled
            logger.LogInformation(LogInfo.From("Migration skipped").WithAdditionalInfo(typeof(TContext).Name));
            return;
        }

        try
        {
            logger.LogInformation(LogInfo.From("Applying database").WithAdditionalInfo(typeof(TContext).Name));
            var dbContext = services.GetRequiredService<TContext>();

            await dbContext.Database.MigrateAsync();
            logger.LogInformation(LogInfo.From("Database migrations applied successfully").WithAdditionalInfo(typeof(TContext).Name));
        }
        catch (Exception ex)
        {
            logger.LogInformation(LogInfo.From("Database migrations error").WithAdditionalInfo(typeof(TContext).Name).ApplyException(ex));
            throw;
        }
    }
    public static WebApplication UseDefaults(this WebApplication app)
    {
        // Use CORS
        app.UseCors(CommonConsts.CorsPolicyName);

        app.UseRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            // c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default OpenAPI");
            c.SwaggerEndpoint($"/swagger/odata/swagger.json", "OData OpenAPI");
            c.SwaggerEndpoint($"/swagger/custom/swagger.json", "Custom OpenAPI");
        });

        // conventional routing for OData
        // https://learn.microsoft.com/en-us/odata/webapi-8/fundamentals/routing-overview?tabs=net60#odata-routing
        app.UseODataRouteDebug();
        app.UseRouting();

        // app.UseAuthentication();
        // app.UseAuthorization();

        // atribute routing for OData
        // https://learn.microsoft.com/en-us/odata/webapi-8/fundamentals/routing-overview?tabs=net60#attribute-routing
        app.MapControllers();
        return app;
    }

    public static IApplicationBuilder UseRequestLogging(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}
