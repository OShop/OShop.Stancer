using Newtonsoft.Json;

namespace OShop.Stancer.Models.Api {
    public class Range {
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("end")]
        public int End { get; set; }
        [JsonProperty("limit")]
        public int Limit { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }
}