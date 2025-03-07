using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OShop.Stancer.Models.Api {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus {
        [EnumMember(Value = "authorized")]
        Authorized,
        [EnumMember(Value = "to_capture")]
        ToCapture,
        [EnumMember(Value = "capture_sent")]
        CaptureSent,
        [EnumMember(Value = "captured")]
        Captured,
        [EnumMember(Value = "disputed")]
        Disputed,
        [EnumMember(Value = "expired")]
        Expired,
        [EnumMember(Value = "failed")]
        Failed,
        [EnumMember(Value = "refused")]
        Refused,
    }
}