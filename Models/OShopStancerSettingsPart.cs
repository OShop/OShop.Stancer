using Orchard.ContentManagement;

namespace OShop.Stancer.Models {
    public class OShopStancerSettingsPart : ContentPart {
        public string PublicKey {
            get { return this.Retrieve(x => x.PublicKey); }
            set { this.Store(x => x.PublicKey, value); }
        }

        public string PrivateKey {
            get { return this.Retrieve(x => x.PrivateKey); }
            set { this.Store(x => x.PrivateKey, value); }
        }
    }
}