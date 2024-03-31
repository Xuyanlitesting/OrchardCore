using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Settings;
using OrchardCore.Sms.Models;
using OrchardCore.Sms.Twilio.Models;

namespace OrchardCore.Sms.Twilio.Services;

public abstract class TwilioSmsProviderBase : ISmsProvider
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
    };

    private readonly ISiteService _siteService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TwilioSmsOptions _providerOptions;
    private readonly ILogger _logger;

    private TwilioSettings _settings;

    protected readonly IStringLocalizer S;

    public TwilioSmsProviderBase(
        ISiteService siteService,
        IHttpClientFactory httpClientFactory,
        TwilioSmsOptions options,
        ILogger logger,
        IStringLocalizer stringLocalizer)
    {
        _siteService = siteService;
        _httpClientFactory = httpClientFactory;
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
            return SmsResult.Failed(S["The Twilio Sms Provider is disabled."]);
        }

        _logger.LogDebug("Attempting to send Sms to {Sms}.", message.To);

        try
        {
            var settings = await GetSettingsAsync();
            var data = new List<KeyValuePair<string, string>>
            {
                new ("From", settings.PhoneNumber),
                new ("To", message.To),
                new ("Body", message.Body),
            };

            var client = GetHttpClient(settings);
            var response = await client.PostAsync($"{settings.AccountSID}/Messages.json", new FormUrlEncodedContent(data));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TwilioMessageResponse>(_jsonSerializerOptions);

                if (string.Equals(result.Status, "sent", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(result.Status, "queued", StringComparison.OrdinalIgnoreCase))
                {
                    return SmsResult.Success;
                }

                _logger.LogError("Twilio service was unable to send SMS messages. Error, code: {errorCode}, message: {errorMessage}", result.ErrorCode, result.ErrorMessage);
            }

            return SmsResult.Failed(S["SMS message was not send."]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending an SMS using the Twilio Sms Provider.");

            return SmsResult.Failed(S["An error occurred while sending an Sms."]);
        }
    }

    private HttpClient GetHttpClient(TwilioSettings settings)
    {
        var token = $"{settings.AccountSID}:{settings.AuthToken}";
        var base64Token = Convert.ToBase64String(Encoding.ASCII.GetBytes(token));

        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Token);

        return client;
    }

    private async Task<TwilioSettings> GetSettingsAsync()
    {
        if (_settings == null)
        {
            var settings = (await _siteService.GetSiteSettingsAsync()).As<TwilioSettings>();

            // It is important to create a new instance of `TwilioSettings` privately to hold the plain auth-token value.
            _settings = new TwilioSettings
            {
                PhoneNumber = settings.PhoneNumber,
                AccountSID = settings.AccountSID,
                AuthToken = settings.AuthToken
            };
        }

        return _settings;
    }
}
