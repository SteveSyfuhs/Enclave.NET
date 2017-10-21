using System;
using System.Collections.Generic;
using System.Text;

namespace Enclave.NET.Configuration
{
    public class Server
    {
        public int Port { get; set; } = EnclaveClient.DefaultPort;

        public ServerCertificate ServerCertificate { get; set; }

        public IEnumerable<Certificate> ClientCertificates { get; set; }
    }
}
