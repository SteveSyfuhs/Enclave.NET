using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enclave.NET
{
    public interface ITokenAuthenticator
    {
        IEnumerable<string> Schemes { get; }

        Task<ClaimsIdentity> Authenticate(IKeyIdentifier id, string token, string scheme = null);
    }
}
