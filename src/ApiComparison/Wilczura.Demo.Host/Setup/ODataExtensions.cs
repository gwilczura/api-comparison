using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Wilczura.Demo.Persistence.Models;

namespace Wilczura.Demo.Host.Setup;

public static class ODataExtensions
{
    public static IHostApplicationBuilder AddOData(
        this IMvcBuilder mvcBuilder, IHostApplicationBuilder app, ILogger logger)
    {
        var modelBuilder = new ODataConventionModelBuilder();

        modelBuilder.EntityType<Country>().HasKey(a => a.CountryId);
        modelBuilder.AddEntitySet<Country>();

        mvcBuilder
            .AddOData(
                options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
                "odata",
                modelBuilder.GetEdmModel()));

        // routing conventions
        // https://github.com/OData/AspNetCoreOData/blob/main/src/Microsoft.AspNetCore.OData/Routing/Conventions/AttributeRoutingConvention.cs

        return app;
    }

    public static EntitySetConfiguration<TEntityType> AddEntitySet<TEntityType>(this ODataModelBuilder modelBuilder, string? enityName = null) where TEntityType : class
    {
        if (string.IsNullOrEmpty(enityName))
        {
            enityName = typeof(TEntityType)
                .Name
                .Replace("Dto", string.Empty)
                .Replace("Model", string.Empty)
                .Replace("Entity", string.Empty);
        }

        var set = modelBuilder.EntitySet<TEntityType>(enityName);
        return set;
    }
}
