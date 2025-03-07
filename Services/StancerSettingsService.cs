using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using OShop.Models;
using OShop.Stancer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OShop.Stancer.Services {
    public class StancerSettingsService : IStancerSettingsService {
        private readonly IEncryptionService _encryptionService;

        public StancerSettingsService(
            IEncryptionService encryptionService,
            IOrchardServices services) {
            _encryptionService = encryptionService;
            Services = services;
        }

        public IOrchardServices Services { get; set; }

        public StancerSettings GetSettings() {
            var settingsPart = Services.WorkContext.CurrentSite.As<OShopStancerSettingsPart>();

            var settings = new StancerSettings() {
                PublicKey = settingsPart.PublicKey,
            };

            if (!string.IsNullOrEmpty(settingsPart.PrivateKey)) {
                settings.PrivateKey = Encoding.UTF8.GetString(_encryptionService.Decode(Convert.FromBase64String(settingsPart.PrivateKey)));
            }

            return settings;
        }

        public void SetSettings(StancerSettings Settings) {
            var settingsPart = Services.WorkContext.CurrentSite.As<OShopStancerSettingsPart>();

            settingsPart.PublicKey = Settings.PublicKey;
            settingsPart.PrivateKey = Convert.ToBase64String(_encryptionService.Encode(Encoding.UTF8.GetBytes(Settings.PrivateKey)));
        }
    }
}