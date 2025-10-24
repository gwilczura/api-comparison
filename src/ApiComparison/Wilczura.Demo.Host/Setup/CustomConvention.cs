using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Wilczura.Demo.Host.Setup;

public class CustomConvention : IControllerModelConvention
{
    //TODO: Swagger Convention for segregting APIs
    public void Apply(ControllerModel controller)
    {
        if (controller.ApiExplorer.GroupName != null)
        {
            return;
        }

        var isOdata = false; // typeof(ODataController).IsAssignableFrom(controller.ControllerType);

        controller.ApiExplorer.GroupName = isOdata ? "odata" : "custom";
    }
}