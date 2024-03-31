using System;
using System.Threading.Tasks;
using Azure.Communication.Sms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Sms.Azure.Models;

namespace OrchardCore.Sms.Azure.Services;

public abstract class AzureSmsProviderBase : ISmsProvider
{
    private readonly AzureSmsOptions _providerOptions;
    private readonly ILogger _logger;

    protected readonly IStringLocalizer S;

    public AzureSmsProviderBase(
        AzureSmsOptions options,
        ILogger logger,
        IStringLocalizer stringLocalizer)
    {
        _providerOptions = options;
        _logger = logger;
        S = stringLocalizer;
    }

    public abstract LocalizedString DisplayName { get; }

    public virtual async Task<SmsResult> SendAsync(SmsMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!_providerOptions.IsEnabled)
        {
            return SmsResult.Failed(S["The Azure Sms Provider is disabled."]);
        }

        _logger.LogDebug("Attempting to send Sms to {Sms}.", message.To);

        try
        {
            var client = new SmsClient(_providerOptions.ConnectionString);
            var smsResult = await client.SendAsync(_providerOptions.PhoneNumber, message.To, message.Body);

            return SmsResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending an SMS using the Azure Sms Provider.");

            return SmsResult.Failed(S["An error occurred while sending an Sms."]);
        }
    }
}
