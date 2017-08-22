using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Enclave.NET.Http.Entities
{
    internal class ClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Claim);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            return ReadClaim(jo);
        }

        public static Claim ReadClaim(JObject jo)
        {
            var type = (string)jo["Type"];
            var value = (string)jo["Value"];
            var valueType = (string)jo["ValueType"];
            var issuer = (string)jo["Issuer"];

            return new Claim(type, value, valueType, issuer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var claim = (Claim)value;

            WriteClaim(writer, claim);
        }

        public static void WriteClaim(JsonWriter writer, Claim claim)
        {
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

    public class ClaimsIdentityConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ClaimsIdentity);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);

            var authType = (string)jo["AuthenticationType"];
            var nameType = (string)jo["NameClaimType"];
            var roleType = (string)jo["RoleClaimType"];

            var claimsArray = (JArray)jo["Claims"];

            var claims = new List<Claim>();

            foreach (var joClaim in claimsArray)
            {
                claims.Add(ClaimConverter.ReadClaim((JObject)joClaim));
            }

            return new ClaimsIdentity(claims, authType, nameType, roleType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var identity = (ClaimsIdentity)value;

            writer.WriteStartObject();

            writer.WritePropertyName("AuthenticationType");
            writer.WriteValue(identity.AuthenticationType);

            writer.WritePropertyName("NameClaimType");
            writer.WriteValue(identity.NameClaimType);

            writer.WritePropertyName("RoleClaimType");
            writer.WriteValue(identity.RoleClaimType);

            writer.WritePropertyName("Claims");

            writer.WriteStartArray();

            foreach (var c in identity.Claims)
            {
                writer.WriteStartObject();

                ClaimConverter.WriteClaim(writer, c);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
