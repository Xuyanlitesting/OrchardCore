using System.Net.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Settings;
using OrchardCore.Sms.Twilio.Models;

namespace OrchardCore.Sms.Twilio.Services;

public class TwilioSmsProvider : TwilioSmsProviderBase
{
    public const string TechnicalName = "Azure";

    public TwilioSmsProvider(
        ISiteService siteService,
        IHttpClientFactory httpClientFactory,
        IOptions<TwilioSmsOptions> options,
        ILogger<TwilioSmsProvider> logger,
        IStringLocalizer<TwilioSmsProvider> stringLocalizer)
        : base(siteService, httpClientFactory, options.Value, logger, stringLocalizer)
    {
    }

    public override LocalizedString DisplayName => S["Twilio Communication Service"];
}
