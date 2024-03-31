namespace OrchardCore.Sms.Twilio.Models;

public class TwilioSmsOptions
{
    public bool IsEnabled { get; set; }

    public string ConnectionString { get; set; }

    public string PhoneNumber { get; set; }

    public bool ConfigurationExists()
        => !string.IsNullOrWhiteSpace(PhoneNumber) && !string.IsNullOrWhiteSpace(ConnectionString);
}
