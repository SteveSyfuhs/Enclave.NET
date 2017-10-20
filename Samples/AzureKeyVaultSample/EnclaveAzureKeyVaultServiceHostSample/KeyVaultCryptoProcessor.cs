using Enclave.NET;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EnclaveAzureKeyVaultServiceHostSample
{
    public class KeyVaultCryptoProcessor : ICryptoProcessor
    {
        public async Task<T> Decrypt<T>(IEnclaveKey key, string ciphertext)
        {
            using (var client = KeyVault.CreateClient())
            {
                var kvKey = key.RetrieveKey<JsonWebKey>();

                var result = await client.DecryptAsync(kvKey.Kid, JsonWebKeyEncryptionAlgorithm.RSAOAEP256, Convert.FromBase64String(ciphertext));

                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result.Result));
            }
        }

        public async Task<string> Encrypt(IEnclaveKey key, object value)
        {
            using (var client = KeyVault.CreateClient())
            {
                var kvKey = key.RetrieveKey<JsonWebKey>();

                var encrypted = await client.EncryptAsync(kvKey.Kid, JsonWebKeyEncryptionAlgorithm.RSAOAEP256, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

                return Convert.ToBase64String(encrypted.Result);
            }
        }

        public async Task<IEnclaveKey> GenerateKey(string keyType)
        {
            using (var client = KeyVault.CreateClient())
            {
                var config = KeyVault.ServiceConfiguration;

                var createdKey = await client.CreateKeyAsync(config.Vault, Guid.NewGuid().ToString(), keyType);

                return new KeyVaultStorageService.KeyVaultKey(createdKey);
            }
        }

        public async Task<string> Sign(IEnclaveKey key, object value)
        {
            using (var client = KeyVault.CreateClient())
            using (var sha = SHA256.Create())
            {
                var kvKey = key.RetrieveKey<JsonWebKey>();

                var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));

                var result = await client.SignAsync(kvKey.Kid, JsonWebKeySignatureAlgorithm.RS256, hashed);

                return Convert.ToBase64String(result.Result);
            }
        }

        public Task<bool> Validate(IEnclaveKey key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
