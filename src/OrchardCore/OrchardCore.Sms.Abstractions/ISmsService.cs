using System.Threading.Tasks;

namespace OrchardCore.Sms;

public interface ISmsService
{
    /// <summary>
    /// Send the given message using the default provider.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="providerName">The technical name of the Email provider. When null or empty, the default provider is used.</param>
    /// <returns>SmsResult object.</returns>
    Task<SmsResult> SendAsync(SmsMessage message, string providerName = null);
}
