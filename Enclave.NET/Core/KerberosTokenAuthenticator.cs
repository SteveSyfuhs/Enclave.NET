using Kerberos.NET;
using Kerberos.NET.Crypto;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enclave.NET.Core
{
    public class KerberosTokenAuthenticator : ITokenAuthenticator
    {
        private readonly IKeyStorageService storage;
        private readonly IDistributedCache cache;

        public KerberosTokenAuthenticator(IKeyStorageService storage, IDistributedCache cache)
        {
            this.storage = storage;
            this.cache = cache;
        }

        public IEnumerable<string> Schemes => new[] { "negotiate", "kerberos" };

        public async Task<ClaimsIdentity> Authenticate(IKeyIdentifier id, string token, string scheme = null)
        {
            var key = await storage.GetKey(id);

            var kerbKey = key.RetrieveKey<byte[]>();

            var validator = new KerberosValidator(
                new KeyTable(
                    new KerberosKey(kerbKey)
                ), 
                cache: cache
            );

            var authenticator = new KerberosAuthenticator(validator);

            return await authenticator.Authenticate(token);
        }
    }
}
