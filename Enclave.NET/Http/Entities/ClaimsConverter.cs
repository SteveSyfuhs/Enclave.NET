using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;

namespace Enclave.NET.Http.Entities
{
    internal class ClaimsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Claim);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var type = (string)jo["Type"];
            var value = (string)jo["Value"];
            var valueType = (string)jo["ValueType"];
            var issuer = (string)jo["Issuer"];

            return new Claim(type, value, valueType, issuer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var claim = (Claim)value;

            writer.WritePropertyName("Type");
            writer.WriteValue(claim.Type);

            writer.WritePropertyName("Value");
            writer.WriteValue(claim.Value);

            writer.WritePropertyName("ValueType");
            writer.WriteValue(claim.ValueType);

            writer.WritePropertyName("Issuer");
            writer.WriteValue(claim.Issuer);
        }
    }
}
