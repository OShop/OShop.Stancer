using Orchard.ContentManagement.Handlers;
using OShop.Stancer.Models;

namespace OShop.Stancer.Handlers {
    public class OShopStancerSettingsPartHandler : ContentHandler {
        public OShopStancerSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<OShopStancerSettingsPart>("Site"));
        }
    }
}