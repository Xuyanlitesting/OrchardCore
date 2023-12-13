using System;
using OrchardCore.Secrets.Models;

namespace OrchardCore.Secrets.Services;

public class SecretActivator<TSecret> : SecretActivator where TSecret : SecretBase, new()
{
    public override Type Type => typeof(TSecret);

    public override SecretBase Create() => new TSecret();
}