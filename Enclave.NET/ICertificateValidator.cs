using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Enclave.NET
{
    public interface ICertificateValidator
    {
        Task<ClaimsIdentity> Validate(X509Certificate2 certificate);
    }
}
