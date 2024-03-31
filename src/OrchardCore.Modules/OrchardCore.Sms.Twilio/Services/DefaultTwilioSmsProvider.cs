using System.Net.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Settings;
using OrchardCore.Sms.Twilio.Models;

namespace OrchardCore.Sms.Twilio.Services;

public class DefaultTwilioSmsProvider : TwilioSmsProviderBase
{
    public const string TechnicalName = "DefaultTwilio";

    public DefaultTwilioSmsProvider(
        ISiteService siteService,
        IHttpClientFactory httpClientFactory,
        IOptions<TwilioSmsOptions> options,
        ILogger<DefaultTwilioSmsProvider> logger,
        IStringLocalizer<DefaultTwilioSmsProvider> stringLocalizer)
        : base(siteService, httpClientFactory, options.Value, logger, stringLocalizer)
    {
    }

    public override LocalizedString DisplayName => S["Default Twilio"];
}
