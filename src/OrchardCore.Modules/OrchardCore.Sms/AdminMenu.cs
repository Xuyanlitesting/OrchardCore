using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Sms.Controllers;

namespace OrchardCore.Sms;

public class AdminMenu : INavigationProvider
{
    private static readonly RouteValueDictionary _routeValues = new()
        {
            { "area", "OrchardCore.Settings" },
            { "groupId", SmsSettings.GroupId },
        };

    protected readonly IStringLocalizer S;

    public AdminMenu(IStringLocalizer<AdminMenu> localizer)
    {
        S = localizer;
    }

    public Task BuildNavigationAsync(string name, NavigationBuilder builder)
    {
        if (!NavigationHelper.IsAdminMenu(name))
        {
            return Task.CompletedTask;
        }

        builder
            .Add(S["Configuration"], configuration => configuration
                .Add(S["Settings"], settings => settings
                   .Add(S["Email"], S["Sms"].PrefixPosition(), entry => entry
                      .AddClass("sms")
                      .Id("sms")
                      .Action("Index", "Admin", _routeValues)
                      .Permission(Permissions.ManageSmsSettings)
                      .LocalNav()
                    )
                   .Add(S["SMS Test"], S["SMS Test"].PrefixPosition(), entry => entry
                      .AddClass("smstest")
                      .Id("smstest")
                      .Action(nameof(AdminController.Test), typeof(AdminController).ControllerName(), "OrchardCore.Sms")
                      .Permission(Permissions.ManageSmsSettings)
                      .LocalNav()
                    )
                )
            );

        return Task.CompletedTask;
    }
}
