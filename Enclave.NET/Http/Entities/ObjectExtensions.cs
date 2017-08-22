using Enclave.NET.Http.Entities;
using Newtonsoft.Json;

namespace System
{
    internal static class ObjectExtensions
    {
        private static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings();

        static ObjectExtensions()
        {
            DefaultSettings.Converters.Add(new ClaimConverter());
            DefaultSettings.Converters.Add(new ClaimsIdentityConverter());
        }

        public static string Serialize(this object obj)
        {
            return Serialize(obj, DefaultSettings);
        }

        public static string Serialize(this object obj, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T Deserialize<T>(this string value)
        {
            return Deserialize<T>(value, DefaultSettings);
        }

        public static T Deserialize<T>(this string value, JsonSerializerSettings settings)
        {
            return (T)JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}
