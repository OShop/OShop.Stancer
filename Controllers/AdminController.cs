using Orchard;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using OShop.Stancer.Models;
using OShop.Stancer.Services;
using OShop.Permissions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OShop.Stancer.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IStancerSettingsService _settingsService;
        private readonly IStancerApiService _stancerApi;

        public AdminController(
            IStancerSettingsService settingsService,
            IStancerApiService stancerApi,
            IOrchardServices services) {
            _settingsService = settingsService;
            _stancerApi = stancerApi;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Settings() {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            return View(_settingsService.GetSettings());
        }

        [HttpPost, FormValueRequired("submit.Save")]
        [ActionName("Settings")]
        public ActionResult SettingsSave(StancerSettings model) {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            if (TryUpdateModel(model)) {
                _settingsService.SetSettings(model);
                Services.Notifier.Information(T("Stancer Settings saved successfully."));
            }
            else {
                Services.Notifier.Error(T("Could not save Stancer Settings."));
            }

            return View(model);
        }

        [HttpPost, FormValueRequired("submit.Validate")]
        [ActionName("Settings")]
        public async Task<ActionResult> SettingsValidate(StancerSettings model) {
            if (!Services.Authorizer.Authorize(OShopPermissions.ManageShopSettings, T("Not allowed to manage Shop Settings")))
                return new HttpUnauthorizedResult();

            if (TryUpdateModel(model)) {
                try {
                    await _stancerApi.PingAsync(model.PrivateKey);
                    Services.Notifier.Information(T("Valid credentials."));
                }
                catch {
                    Services.Notifier.Warning(T("Invalid credentials."));
                }
            }
            else {
                Services.Notifier.Error(T("Could not validate credentials."));
            }

            return View(model);
        }

    }
}