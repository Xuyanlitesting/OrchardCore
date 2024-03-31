namespace OrchardCore.Sms.Twilio.Models;

public class TwilioSmsSettings
{
    public bool IsEnabled { get; set; }

    public string ConnectionString { get; set; }

    public string PhoneNumber { get; set; }
}
