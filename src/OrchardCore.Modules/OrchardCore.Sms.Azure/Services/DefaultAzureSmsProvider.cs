using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Sms.Azure.Models;

namespace OrchardCore.Sms.Azure.Services;

public class DefaultAzureSmsProvider : AzureSmsProviderBase
{
    public const string TechnicalName = "DefaultAzure";

    public DefaultAzureSmsProvider(
        IOptions<DefaultAzureSmsOptions> options,
        ILogger<DefaultAzureSmsProvider> logger,
        IStringLocalizer<DefaultAzureSmsProvider> stringLocalizer)
        : base(options.Value, logger, stringLocalizer)
    {
    }

    public override LocalizedString DisplayName => S["Default Azure Communication Service"];
}
