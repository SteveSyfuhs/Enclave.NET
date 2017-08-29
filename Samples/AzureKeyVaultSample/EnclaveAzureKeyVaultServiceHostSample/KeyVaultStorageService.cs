using System.Linq;
using System.Threading.Tasks;
using Enclave.NET;
using Microsoft.Azure.KeyVault;
using System;
using Microsoft.Azure.KeyVault.Models;
using System.Text;

namespace EnclaveAzureKeyVaultServiceHostSample
{
    internal class KeyVaultStorageService : IKeyStorageService
    {
        private class KeyVaultKeyIdentifier : IKeyIdentifier
        {
            public KeyVaultKeyIdentifier(string keyId)
            {
                this.KeyId = Convert.ToBase64String(Encoding.UTF8.GetBytes(keyId));
            }

            public string KeyId { get; }
        }

        internal class KeyVaultKey : IEnclaveKey
        {
            private readonly KeyBundle key;

            public KeyVaultKey(KeyBundle key)
            {
                this.key = key;
            }

            public string KeyType => key.Key.Kty;

            public string Identifier => key.KeyIdentifier.Identifier;

            public object RetrieveKey()
            {
                return key.Key;
            }

            public T RetrieveKey<T>()
            {
                return (T)RetrieveKey();
            }
        }

        public Task<IKeyIdentifier> AddKey(IEnclaveKey key)
        {
            var kvKey = (KeyVaultKey)key;

            IKeyIdentifier id = new KeyVaultKeyIdentifier(kvKey.Identifier);

            return Task.FromResult(id);
        }

        public async Task<IEnclaveKey> GetKey(IKeyIdentifier id)
        {
            var client = KeyVault.CreateClient();

            var keyId = Encoding.UTF8.GetString(Convert.FromBase64String(id.KeyId));

            var key = await client.GetKeyAsync(keyId);

            return new KeyVaultKey(key);
        }

        public async Task<IQueryable<IKeyIdentifier>> ListKeys()
        {
            var client = KeyVault.CreateClient();

            var config = KeyVault.ServiceConfiguration;

            var keys = await client.GetKeysAsync(config.Vault);

            return keys.Select(k => new KeyVaultKeyIdentifier(k.Identifier.Identifier)).AsQueryable();
        }
    }
}