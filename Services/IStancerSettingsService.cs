using Orchard;
using OShop.Stancer.Models;

namespace OShop.Stancer.Services {
    public interface IStancerSettingsService : IDependency {
        StancerSettings GetSettings();
        void SetSettings(StancerSettings Settings);
    }
}
