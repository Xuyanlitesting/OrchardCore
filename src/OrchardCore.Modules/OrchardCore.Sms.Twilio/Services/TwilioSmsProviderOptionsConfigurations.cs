using Microsoft.Extensions.Options;
using OrchardCore.Sms.Twilio.Models;

namespace OrchardCore.Sms.Twilio.Services;

public class TwilioSmsProviderOptionsConfigurations : IConfigureOptions<SmsProviderOptions>
{
    private readonly TwilioSmsOptions _TwilioOptions;
    private readonly DefaultTwilioSmsOptions _defaultTwilioOptions;

    public TwilioSmsProviderOptionsConfigurations(
        IOptions<TwilioSmsOptions> TwilioOptions,
        IOptions<DefaultTwilioSmsOptions> defaultTwilioOptions)
    {
        _TwilioOptions = TwilioOptions.Value;
        _defaultTwilioOptions = defaultTwilioOptions.Value;
    }

    public void Configure(SmsProviderOptions options)
    {
        ConfigureTenantProvider(options);

        if (_defaultTwilioOptions.IsEnabled)
        {
            // Only configure the default provider, if settings are provided by the configuration provider.
            ConfigureDefaultProvider(options);
        }
    }

    private void ConfigureTenantProvider(SmsProviderOptions options)
    {
        var typeOptions = new SmsProviderTypeOptions(typeof(TwilioSmsProvider))
        {
            IsEnabled = _TwilioOptions.IsEnabled,
        };

        options.TryAddProvider(TwilioSmsProvider.TechnicalName, typeOptions);
    }

    private static void ConfigureDefaultProvider(SmsProviderOptions options)
    {
        var typeOptions = new SmsProviderTypeOptions(typeof(DefaultTwilioSmsProvider))
        {
            IsEnabled = true,
        };

        options.TryAddProvider(DefaultTwilioSmsProvider.TechnicalName, typeOptions);
    }
}
