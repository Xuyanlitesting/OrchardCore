using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using OrchardCore.Settings;
using OrchardCore.Sms.Twilio.Models;

namespace OrchardCore.Sms.Twilio.Services;

public class TwilioSmsOptionsConfiguration : IConfigureOptions<TwilioSmsOptions>
{
    public const string ProtectorName = "TwilioSmsProtector";

    private readonly ISiteService _siteService;
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public TwilioSmsOptionsConfiguration(
        ISiteService siteService,
        IDataProtectionProvider dataProtectionProvider)
    {
        _siteService = siteService;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public void Configure(TwilioSmsOptions options)
    {
        var settings = _siteService.GetSiteSettingsAsync()
            .GetAwaiter()
            .GetResult()
            .As<TwilioSmsSettings>();

        options.IsEnabled = settings.IsEnabled;
        options.PhoneNumber = settings.PhoneNumber;

        if (!string.IsNullOrEmpty(settings.ConnectionString))
        {
            var protector = _dataProtectionProvider.CreateProtector(ProtectorName);

            options.ConnectionString = protector.Unprotect(settings.ConnectionString);
        }
    }
}
