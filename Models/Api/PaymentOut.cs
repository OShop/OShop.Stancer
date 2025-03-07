using System;
using Newtonsoft.Json;
using OShop.Stancer.Converters;

namespace OShop.Stancer.Models.Api {
    public class PaymentOut {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("capture")]
        public bool Capture { get; set; }

        [JsonProperty("created"), JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime Created { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("date_paym"), JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime? DatePayment { get; set; }

        [JsonProperty("date_settlement"), JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime? DateSettlement { get; set; }

        [JsonProperty("date_bank"), JsonConverter(typeof(UnixSecondsDateTimeConverter))]
        public DateTime? DateBank { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("method")]
        public PaymentMethods? Method { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        [JsonProperty("return_url")]
        public string ReturnUrl { get; set; }

        [JsonProperty("status")]
        public PaymentStatus? Status { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}