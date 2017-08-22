using System.Threading.Tasks;

namespace Enclave.NET
{
    public interface ICryptoProcessor
    {
        Task<IEnclaveKey> GenerateKey(string keyType);

        Task<string> Encrypt(IEnclaveKey key, object value);

        Task<T> Decrypt<T>(IEnclaveKey key, string ciphertext);

        Task<string> Sign(IEnclaveKey key, object value);

        Task<bool> Validate(IEnclaveKey key, string value);
    }
}
