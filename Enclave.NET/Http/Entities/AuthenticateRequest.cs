
namespace Enclave.NET.Http.Entities
{
    public class AuthenticateResult
    {
        public string Identity { get; set; }
    }

    public class AuthenticateRequest
    {
        public string Token { get; set; }

        public string Scheme { get; set; }
    }
}
