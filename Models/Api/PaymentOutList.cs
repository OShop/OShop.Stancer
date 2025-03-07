using System.Collections.Generic;
using Newtonsoft.Json;

namespace OShop.Stancer.Models.Api {
    public class PaymentOutList {
        [JsonProperty("range")]
        public Range Range { get; set; }

        [JsonProperty("payments")]
        public IEnumerable<PaymentOut> Payments { get; set; }
    }
}