using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Sms.Azure.Models;

namespace OrchardCore.Sms.Azure.Services;

public class AzureSmsProvider : AzureSmsProviderBase
{
    public const string TechnicalName = "Azure";

    public AzureSmsProvider(
        IOptions<AzureSmsOptions> options,
        ILogger<AzureSmsProvider> logger,
        IStringLocalizer<AzureSmsProvider> stringLocalizer)
        : base(options.Value, logger, stringLocalizer)
    {
    }

    public override LocalizedString DisplayName => S["Azure Communication Service"];
}
