using Enclave.NET;
using Enclave.NET.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace AzureKeyVaultSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1500);

            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("settings.json")
                                    .AddEnvironmentVariables().Build();

            var server = configuration.GetSection("Server").Get<Server>();

            var client = new EnclaveClient(
                    FindTestCertificate(server),
                    pinnedCertificates: new[] {
                        new Certificate { Thumbprint = server.ServerCertificate.Thumbprint }
                    }
                );

            var data = new { Protected = "super-secret-secret-stuff" };

            Console.WriteLine($"Data: " + JsonConvert.SerializeObject(data));

            var key = await client.GenerateKey("RSA");

            Console.WriteLine($"KeyId: {key.KeyId}");

            var encrypted = await client.Encrypt(key, data);

            Console.WriteLine($"Encrypted: {encrypted.Value}");

            var decrypted = await client.Decrypt<object>(key, encrypted.Value);

            Console.WriteLine($"Decrypted: {decrypted}");

            var signed = await client.Sign(key, data);

            Console.WriteLine($"Signed: {signed}");

            Console.ReadKey();
        }

        private static X509Certificate2 FindTestCertificate(Server server)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                foreach (var cert in server.ClientCertificates)
                {
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, cert.Thumbprint, false);

                    if (certs.Count == 1)
                    {
                        return certs[0];
                    }
                }

                return null;
            }
            finally
            {
                store.Close();
            }
        }
    }
}
