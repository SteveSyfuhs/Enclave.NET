using System.Security.Claims;
using System.Threading.Tasks;

namespace Enclave.NET
{
    public interface ISchemeAuthenticator
    {
        Task<ClaimsIdentity> Authenticate(IKeyIdentifier id, string token, string scheme);
    }
}
