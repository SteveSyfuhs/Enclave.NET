using Newtonsoft.Json;
using System.Diagnostics;

namespace Enclave.NET
{
    [DebuggerDisplay("{KeyId}")]
    public class KeyIdentifier : IKeyIdentifier
    {
        [JsonProperty]
        public string KeyId { get; private set; }

        public static IKeyIdentifier Parse(string id)
        {
            return new KeyIdentifier { KeyId = id };
        }
    }
}