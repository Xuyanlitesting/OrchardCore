using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using OrchardCore.Sms.Drivers;

namespace OrchardCore.Sms;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddSmsServices()
            .AddScoped<IDisplayDriver<ISite>, SmsSettingsDisplayDriver>()
            .AddScoped<IPermissionProvider, Permissions>()
            .AddScoped<INavigationProvider, AdminMenu>();
    }
}
