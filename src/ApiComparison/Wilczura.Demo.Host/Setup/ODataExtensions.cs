using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Wilczura.Demo.Host.Setup;

public static class ODataExtensions
{
    public static IHostApplicationBuilder AddOData(
        this IMvcBuilder mvcBuilder, IHostApplicationBuilder app, ILogger logger)
    {
        var modelBuilder = new ODataConventionModelBuilder();

        mvcBuilder
            .AddOData(
                options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
                "odata",
                modelBuilder.GetEdmModel()));

        // routing conventions
        // https://github.com/OData/AspNetCoreOData/blob/main/src/Microsoft.AspNetCore.OData/Routing/Conventions/AttributeRoutingConvention.cs

        return app;
    }
}
