using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Settings;
using OrchardCore.Sms.Twilio.Drivers;
using OrchardCore.Sms.Twilio.Models;
using OrchardCore.Sms.Twilio.Services;

namespace OrchardCore.Sms.Twilio;

public class Startup
{
    private readonly IShellConfiguration _shellConfiguration;

    public Startup(IShellConfiguration shellConfiguration)
    {
        _shellConfiguration = shellConfiguration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<TwilioSmsOptions>, TwilioSmsOptionsConfiguration>();

        services.AddSmsProviderOptionsConfiguration<TwilioSmsProviderOptionsConfigurations>()
            .AddScoped<IDisplayDriver<ISite>, TwilioSmsSettingsDisplayDriver>();

        services.Configure<DefaultTwilioSmsOptions>(options =>
        {
            _shellConfiguration.GetSection("OrchardCore_Sms_Twilio").Bind(options);

            options.IsEnabled = options.ConfigurationExists();
        });
    }
}
