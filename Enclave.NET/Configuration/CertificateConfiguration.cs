using System.Security.Cryptography.X509Certificates;

namespace Enclave.NET.Configuration
{
    public class CertificateConfiguration
    {
        public StoreName StoreName { get; set; }

        public StoreLocation StoreLocation { get; set; }

        public string Thumbprint { get; set; }
    }
}
