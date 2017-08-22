using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enclave.NET.Core
{
    public class SchemeAuthenticator : ISchemeAuthenticator
    {
        private readonly ConcurrentDictionary<string, ITokenAuthenticator> authenticators;

        private readonly string defaultScheme;

        public SchemeAuthenticator(IEnumerable<ITokenAuthenticator> authenticators)
        {
            this.authenticators = new ConcurrentDictionary<string, ITokenAuthenticator>();

            foreach (var authenticator in authenticators)
            {
                foreach (var scheme in authenticator.Schemes)
                {
                    var schemeLower = scheme.ToLowerInvariant();

                    this.authenticators[schemeLower] = authenticator;

                    if (string.IsNullOrWhiteSpace(defaultScheme))
                    {
                        defaultScheme = schemeLower;
                    }
                }
            }
        }

        public IEnumerable<string> Schemes => new string[0];

        public async Task<ClaimsIdentity> Authenticate(IKeyIdentifier id, string token, string scheme)
        {
            var authenticator = TryFindAuthenticator(token, scheme);

            if (authenticator == null)
            {
                return null;
            }

            if (typeof(SchemeAuthenticator) == authenticator.GetType())
            {
                return null;
            }

            return await authenticator.Authenticate(id, token, scheme);
        }

        private ITokenAuthenticator TryFindAuthenticator(string token, string scheme)
        {
            if (authenticators.TryGetValue(scheme.ToLowerInvariant(), out ITokenAuthenticator authenticator))
            {
                return authenticator;
            }

            if (authenticators.TryGetValue(defaultScheme, out authenticator))
            {
                return authenticator;
            }

            return null;
        }
    }
}
