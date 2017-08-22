using System.Linq;
using System.Threading.Tasks;

namespace Enclave.NET
{
    public interface IKeyStorageService
    {
        Task<IKeyIdentifier> AddKey(IEnclaveKey key);

        Task<IEnclaveKey> GetKey(IKeyIdentifier id);

        Task<IQueryable<IKeyIdentifier>> ListKeys();
    }
}
