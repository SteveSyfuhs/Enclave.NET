using Enclave.NET.Http.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Enclave.NET
{
    public interface IEnclaveService
    {
        Task<EncryptedResult> Encrypt(IKeyIdentifier id, object value);

        Task<T> Decrypt<T>(IKeyIdentifier id, string ciphertext);

        Task<string> Sign(IKeyIdentifier id, object value);

        Task<bool> Validate(IKeyIdentifier id, string signed);

        Task<IKeyIdentifier> GenerateKey(string keyType);

        IKeyIdentifier ParseKeyId(string id);

        Task<IQueryable<IKeyIdentifier>> ListKeys();

        Task<ClaimsIdentity> AuthenticateToken(IKeyIdentifier id, string scheme, string token);
    }
}
