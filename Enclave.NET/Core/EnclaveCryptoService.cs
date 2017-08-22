using Enclave.NET.Http.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Enclave.NET.Core
{
    public class EnclaveCryptoService : IEnclaveService
    {
        private readonly ICryptoProcessor processor;
        private readonly IKeyStorageService storage;
        private readonly ISchemeAuthenticator authenticator;

        public EnclaveCryptoService(ICryptoProcessor processor, IKeyStorageService storage, ISchemeAuthenticator authenticator)
        {
            this.processor = processor;
            this.storage = storage;
            this.authenticator = authenticator;
        }

        public async Task<T> Decrypt<T>(IKeyIdentifier id, string ciphertext)
        {
            var key = await storage.GetKey(id);

            var decrypted = await processor.Decrypt<T>(key, ciphertext);

            return decrypted;
        }

        public async Task<EncryptedResult> Encrypt(IKeyIdentifier id, object value)
        {
            var key = await storage.GetKey(id);

            var encrypted = await processor.Encrypt(key, value);

            return new EncryptedResult { Value = encrypted };
        }

        public async Task<IKeyIdentifier> GenerateKey(string keyType)
        {
            var key = await processor.GenerateKey(keyType);

            return await storage.AddKey(key);
        }

        public async Task<string> Sign(IKeyIdentifier id, object value)
        {
            var key  = await storage.GetKey(id);

            return await processor.Sign(key, value);
        }

        public async Task<bool> Validate(IKeyIdentifier id, string signed)
        {
            var key = await storage.GetKey(id);

            return await processor.Validate(key, signed);
        }

        public IKeyIdentifier ParseKeyId(string id)
        {
            return KeyIdentifier.Parse(id);
        }

        public async Task<IQueryable<IKeyIdentifier>> ListKeys()
        {
            return await storage.ListKeys();
        }

        public async Task<ClaimsIdentity> AuthenticateToken(IKeyIdentifier id, string scheme, string token)
        {
            return await authenticator.Authenticate(id, token, scheme);
        }
    }
}
