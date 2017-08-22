using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Enclave.NET.Core
{
    internal class InMemoryStorageService : IKeyStorageService
    {
        private static readonly Dictionary<string, IEnclaveKey> InMemoryKeys = new Dictionary<string, IEnclaveKey>();

        private static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        public Task<IKeyIdentifier> AddKey(IEnclaveKey key)
        {
            var idBytes = new byte[16];

            RNG.GetBytes(idBytes);

            IKeyIdentifier keyId = new InMemoryKeyIdentifier(ToBase64UrlString(idBytes));

            InMemoryKeys[keyId.KeyId] = key;

            return Task.FromResult(keyId);
        }

        public Task<IEnclaveKey> GetKey(IKeyIdentifier id)
        {
            if (InMemoryKeys.TryGetValue(id.KeyId, out IEnclaveKey key))
            {
                return Task.FromResult(key);
            }

            throw new InvalidOperationException($"Key with Id {id.KeyId} not found");
        }

        private static string ToBase64UrlString(byte[] input)
        {
            var result = new StringBuilder(Convert.ToBase64String(input).TrimEnd('='));

            result.Replace('+', '-');
            result.Replace('/', '_');

            return result.ToString();
        }

        public Task<IQueryable<IKeyIdentifier>> ListKeys()
        {
            var keys = InMemoryKeys.Keys.Select(k => KeyIdentifier.Parse(k)).AsQueryable();

            return Task.FromResult(keys);
        }

        private class InMemoryKeyIdentifier : IKeyIdentifier
        {
            public InMemoryKeyIdentifier(string keyId)
            {
                KeyId = keyId;
            }

            public string KeyId { get; }
        }
    }
}
