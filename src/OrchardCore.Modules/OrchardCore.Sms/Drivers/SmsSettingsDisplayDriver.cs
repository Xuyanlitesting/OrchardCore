using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using OrchardCore.Settings;
using OrchardCore.Sms.ViewModels;

namespace OrchardCore.Sms.Drivers;

public class SmsSettingsDisplayDriver : SectionDisplayDriver<ISite, SmsSettings>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly IShellHost _shellHost;
    private readonly SmsOptions _smsOptions;
    private readonly ISmsProviderResolver _SmsProviderResolver;
    private readonly ShellSettings _shellSettings;
    private readonly SmsProviderOptions _smsProviders;

    protected readonly IStringLocalizer S;

    public SmsSettingsDisplayDriver(
        IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService,
        IShellHost shellHost,
        IOptions<SmsProviderOptions> SmsProviders,
        IOptions<SmsOptions> SmsOptions,
        ISmsProviderResolver SmsProviderResolver,
        ShellSettings shellSettings,
        IStringLocalizer<SmsSettingsDisplayDriver> stringLocalizer)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
        _shellHost = shellHost;
        _smsOptions = SmsOptions.Value;
        _SmsProviderResolver = SmsProviderResolver;
        _smsProviders = SmsProviders.Value;
        _shellSettings = shellSettings;
        S = stringLocalizer;
    }
    public override async Task<IDisplayResult> EditAsync(SmsSettings settings, BuildEditorContext context)
    {
        if (!context.GroupId.EqualsOrdinalIgnoreCase(SmsSettings.GroupId))
        {
            return null;
        }

        if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext?.User, Permissions.ManageSmsSettings))
        {
            return null;
        }

        context.Shape.Metadata.Wrappers.Add("Settings_Wrapper__Reload");

        return Initialize<SmsSettingsViewModel>("SmsSettings_Edit", async model =>
        {
            model.DefaultProvider = settings.DefaultProviderName ?? _smsOptions.DefaultProviderName;
            model.Providers = await GetProviderOptionsAsync();
        }).Location("Content:1#Providers")
        .OnGroup(SmsSettings.GroupId);
    }

    public override async Task<IDisplayResult> UpdateAsync(SmsSettings settings, BuildEditorContext context)
    {
        if (!context.GroupId.EqualsOrdinalIgnoreCase(SmsSettings.GroupId))
        {
            return null;
        }

        if (!await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext?.User, Permissions.ManageSmsSettings))
        {
            return null;
        }

        var model = new SmsSettingsViewModel();

        if (await context.Updater.TryUpdateModelAsync(model, Prefix))
        {
            if (settings.DefaultProviderName != model.DefaultProvider)
            {
                settings.DefaultProviderName = model.DefaultProvider;

                await _shellHost.ReleaseShellContextAsync(_shellSettings);
            }
        }

        return await EditAsync(settings, context);
    }

    private async Task<SelectListItem[]> GetProviderOptionsAsync()
    {
        var options = new List<SelectListItem>();

        foreach (var entry in _smsProviders.Providers)
        {
            if (!entry.Value.IsEnabled)
            {
                continue;
            }

            var provider = await _SmsProviderResolver.GetAsync(entry.Key);

            options.Add(new SelectListItem(provider.DisplayName, entry.Key));
        }

        return options.OrderBy(x => x.Text).ToArray();
    }
}
