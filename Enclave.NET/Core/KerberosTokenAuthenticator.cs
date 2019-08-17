using Kerberos.NET;
using Kerberos.NET.Crypto;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enclave.NET.Core
{
    public class KerberosTokenAuthenticator : ITokenAuthenticator
    {
        private readonly IKeyStorageService storage;

        public KerberosTokenAuthenticator(IKeyStorageService storage)
        {
            this.storage = storage;
        }

        public IEnumerable<string> Schemes => new[] { "negotiate", "kerberos" };

        public async Task<ClaimsIdentity> Authenticate(IKeyIdentifier id, string token, string scheme = null)
        {
            var key = await storage.GetKey(id);

            var kerbKey = key.RetrieveKey<byte[]>();

            var validator = new KerberosValidator(
                new KeyTable(
                    new KerberosKey(kerbKey)
                )
            );

            var authenticator = new KerberosAuthenticator(validator);

            return await authenticator.Authenticate(token);
        }
    }
}
