using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace OShop.Stancer.Converters {
    public class UnixSecondsDateTimeConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            if (objectType == typeof(DateTime)) {
                return true;
            }
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                return Nullable.GetUnderlyingType(objectType) == typeof(DateTime);
            }
            else {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) {
                if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    return null;
                }
                else {
                    throw new ArgumentException(String.Format("Could not convert null value to {0}.", objectType));
                }
            }
            long lValue;
            if (long.TryParse(reader.Value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out lValue)) {
                return DateTimeOffset.FromUnixTimeSeconds(lValue).DateTime;
            }
            else {
                throw new ArgumentException(String.Format("Could not convert {0} value to long.", reader.Value.ToString()));
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
            }
            else {
                writer.WriteValue(new DateTimeOffset((DateTime)value).ToUnixTimeSeconds());
            }
        }
    }
}