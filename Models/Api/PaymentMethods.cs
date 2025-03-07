using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OShop.Stancer.Models.Api {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentMethods {
        [EnumMember(Value = "card")]
        Card,
        [EnumMember(Value = "sepa")]
        Sepa
    }
}