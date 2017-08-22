using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Enclave.NET.Core
{
    internal class InMemoryCryptoProcessor : ICryptoProcessor
    {
        private static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();

        public Task<T> Decrypt<T>(IEnclaveKey key, string ciphertext)
        {
            T result = Jose.JWT.Decode<T>(ciphertext, key.RetrieveKey());

            return Task.FromResult(result);
        }

        public Task<string> Encrypt(IEnclaveKey key, object value)
        {
            Jose.JweAlgorithm algorithm;
            Jose.JweEncryption encryption;

            switch (key.KeyType.ToLowerInvariant())
            {
                case "rsa":
                    algorithm = Jose.JweAlgorithm.RSA_OAEP;
                    encryption = Jose.JweEncryption.A128CBC_HS256;
                    break;
                case "aes":
                    algorithm = Jose.JweAlgorithm.A128KW;
                    encryption = Jose.JweEncryption.A128CBC_HS256;
                    break;
                case "ecc":
                    algorithm = Jose.JweAlgorithm.ECDH_ES;
                    encryption = Jose.JweEncryption.A128CBC_HS256;
                    break;
                default:
                    throw new NotSupportedException($"Unknown KeyType {key.KeyType}");
            }

            var encrypted = Jose.JWT.Encode(value, key.RetrieveKey(), algorithm, encryption);

            return Task.FromResult(encrypted);
        }

        public Task<IEnclaveKey> GenerateKey(string keyType)
        {
            IEnclaveKey key = null;

            switch (keyType.ToLowerInvariant())
            {
                case "rsa":
                    key = new InMemoryKey(keyType, new RSACryptoServiceProvider(2048));
                    break;
                case "aes":
                    var bytes = new byte[16];
                    RNG.GetBytes(bytes);

                    key = new InMemoryKey(keyType, bytes);
                    break;
                case "ecc":
                    key = new InMemoryKey(keyType, new ECDsaCng(256));
                    break;
                default:
                    throw new NotSupportedException($"Unknown key type {keyType}");
            }

            return Task.FromResult(key);
        }

        public Task<string> Sign(IEnclaveKey key, object value)
        {
            Jose.JwsAlgorithm algorithm;

            switch (key.KeyType.ToLowerInvariant())
            {
                case "rsa":
                    algorithm = Jose.JwsAlgorithm.RS256;
                    break;
                case "aes":
                    algorithm = Jose.JwsAlgorithm.HS256;
                    break;
                case "ecc":
                    algorithm = Jose.JwsAlgorithm.ES256;
                    break;
                default:
                    throw new NotSupportedException($"Unknown KeyType {key.KeyType}");
            }

            var result = Jose.JWT.Encode(value, key.RetrieveKey(), algorithm);

            return Task.FromResult(result);
        }

        public Task<bool> Validate(IEnclaveKey key, string value)
        {
            var result = Jose.JWT.Decode(value, key.RetrieveKey());

            return Task.FromResult(!string.IsNullOrWhiteSpace(result));
        }

        public class InMemoryKey : IEnclaveKey
        {
            private readonly object key;

            public InMemoryKey(string keyType, object key)
            {
                KeyType = keyType;
                this.key = key;
            }

            public string KeyType { get; private set; }

            public object RetrieveKey()
            {
                return key;
            }

            public T RetrieveKey<T>()
            {
                return (T)RetrieveKey();
            }
        }
    }
}
