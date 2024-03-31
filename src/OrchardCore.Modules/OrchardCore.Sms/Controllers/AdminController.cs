using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Sms.ViewModels;

namespace OrchardCore.Sms.Controllers;

public class AdminController : Controller
{
    private readonly IAuthorizationService _authorizationService;
    private readonly INotifier _notifier;
    private readonly SmsOptions _smsOptions;
    private readonly SmsProviderOptions _providerOptions;
    private readonly ISmsService _smsService;
    private readonly ISmsProviderResolver _smsProviderResolver;

    protected readonly IHtmlLocalizer H;
    protected readonly IStringLocalizer S;

    public AdminController(
        IAuthorizationService authorizationService,
        INotifier notifier,
        IOptions<SmsProviderOptions> providerOptions,
        IOptions<SmsOptions> SmsOptions,
        ISmsService SmsService,
        ISmsProviderResolver SmsProviderResolver,
        IHtmlLocalizer<AdminController> htmlLocalizer,
        IStringLocalizer<AdminController> stringLocalizer)
    {
        _authorizationService = authorizationService;
        _notifier = notifier;
        _smsOptions = SmsOptions.Value;
        _providerOptions = providerOptions.Value;
        _smsService = SmsService;
        _smsProviderResolver = SmsProviderResolver;
        H = htmlLocalizer;
        S = stringLocalizer;
    }

    [Admin("Sms/Test", "SmsTest")]
    public async Task<IActionResult> Test()
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSmsSettings))
        {
            return Forbid();
        }

        var model = new SmsTestViewModel()
        {
            Provider = _smsOptions.DefaultProviderName,
        };

        PopulateModel(model);

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Test(SmsTestViewModel model)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSmsSettings))
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            var message = new SmsMessage()
            {
                To = model.PhoneNumber,
                Body = S["This is a test SMS message."]
            };

            try
            {
                var result = await _smsService.SendAsync(message, model.Provider);

                if (result.Succeeded)
                {
                    await _notifier.SuccessAsync(H["The test SMS message has been successfully sent."]);

                    return RedirectToAction(nameof(Test));
                }
                else
                {
                    await _notifier.ErrorAsync(H["The test SMS message failed to send."]);
                }
            }
            catch (InvalidSmsProviderException)
            {
                ModelState.AddModelError(string.Empty, S["The selected provider is invalid or no longer enabled."]);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, S["Unable to send the message using the selected provider."]);
            }
        }

        PopulateModel(model);

        return View(model);
    }

    private async void PopulateModel(SmsTestViewModel model)
    {
        var options = new List<SelectListItem>();

        foreach (var entry in _providerOptions.Providers)
        {
            if (!entry.Value.IsEnabled)
            {
                continue;
            }

            var provider = await _smsProviderResolver.GetAsync(entry.Key);

            options.Add(new SelectListItem(provider.DisplayName, entry.Key));
        }

        model.Providers = options.OrderBy(x => x.Text).ToArray();
    }
}
