using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OrchardCore.Sms.Azure.ViewModels;

public class AzureSettingsViewModel
{
    public bool IsEnabled { get; set; }

    public string ConnectionString { get; set; }

    public string PhoneNumber { get; set; }

    [BindNever]
    public bool HasConnectionString { get; set; }
}
