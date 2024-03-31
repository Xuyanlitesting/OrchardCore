using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace OrchardCore.Sms;

public class SmsProviderTypeOptions
{
    public Type Type { get; }

    public SmsProviderTypeOptions(Type type)
    {
        if (!typeof(ISmsProvider).IsAssignableFrom(type))
        {
            throw new ArgumentException($"The type must implement the '{nameof(ISmsProvider)}' interface.");
        }

        Type = type;
    }

    public bool IsEnabled { get; set; }
}
