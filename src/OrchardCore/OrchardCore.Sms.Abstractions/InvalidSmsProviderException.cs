using System;
namespace OrchardCore.Sms;

public class InvalidSmsProviderException : ArgumentOutOfRangeException
{
    public InvalidSmsProviderException(string name)
        : base(nameof(name), $"'{name}' is an invalid SMS provider name.")
    {
    }
}
