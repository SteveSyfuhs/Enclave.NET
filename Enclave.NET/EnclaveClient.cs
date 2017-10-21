using Enclave.NET.Http.Entities;
using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net.Security;
using Enclave.NET.Configuration;
using Enclave.NET.Http.Authentication;

namespace Enclave.NET
{
    public class EnclaveClient : IEnclaveService
    {
        public const int DefaultPort = 44320;

        private readonly HttpClient client;

        public EnclaveClient(X509Certificate2 certificate, int port = DefaultPort, IEnumerable<Certificate> pinnedCertificates = null)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            if (port <= 0 || port >= ushort.MaxValue)
            {
                port = DefaultPort;
            }

            if (pinnedCertificates == null)
            {
                pinnedCertificates = new Certificate[0];
            }

            var handler = new HttpClientHandler()
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };

            if (pinnedCertificates.Any())
            {
                handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => ValidateServiceCertificate(a, b, c, d, pinnedCertificates);
            }

            handler.ClientCertificates.Add(certificate);

            client = new HttpClient(handler) { BaseAddress = new Uri($"https://localhost:{port}/") };
        }

        protected virtual bool ValidateServiceCertificate(
            HttpRequestMessage message,
            X509Certificate2 certificate,
            X509Chain chain,
            SslPolicyErrors errors,
            IEnumerable<Certificate> pinned
        )
        {
            return CertificateHandler.ValidateCertificate(certificate, chain, errors, pinned);
        }

        public async Task<ClaimsIdentity> AuthenticateToken(IKeyIdentifier id, string scheme, string token)
        {
            var result = await Execute<AuthenticateResult>(id, "authenticate", new AuthenticateRequest { Token = token, Scheme = scheme });

            return result.Identity.Deserialize<ClaimsIdentity>();
        }

        public Task<T> Decrypt<T>(IKeyIdentifier id, string ciphertext)
        {
            return Execute<T>(id, "decrypt", new { Value = ciphertext });
        }

        public Task<EncryptedResult> Encrypt(IKeyIdentifier id, object value)
        {
            return Execute<EncryptedResult>(id, "encrypt", new EncryptRequest { Value = value });
        }

        public async Task<IKeyIdentifier> GenerateKey(string keyType)
        {
            return await Execute<KeyIdentifier>(null, "generate", new GenerateRequest { KeyType = keyType });
        }

        public async Task<IQueryable<IKeyIdentifier>> ListKeys()
        {
            var keys = await Execute<IEnumerable<KeyIdentifier>>();

            return keys.AsQueryable();
        }

        public IKeyIdentifier ParseKeyId(string id)
        {
            return KeyIdentifier.Parse(id);
        }

        public async Task<string> Sign(IKeyIdentifier id, object value)
        {
            var response = await Execute<SignResult>(id, "sign", new SignRequest { Value = value });

            return response.Value;
        }

        public async Task<bool> Validate(IKeyIdentifier id, string signed)
        {
            var result = await Execute<ValidateResult>(id, "validate", new { Value = signed });

            return result.Result;
        }

        private async Task<T> Execute<T>(IKeyIdentifier id = null, string operation = null, object body = null)
        {
            var path = "/keys/";

            if (id != null)
            {
                path += $"{id.KeyId}/";
            }

            path += operation;

            HttpResponseMessage response;

            if (body != null)
            {
                response = await client.PostAsync(path, new StringContent(body.Serialize(), Encoding.UTF8, "application/json"));
            }
            else
            {
                response = await client.GetAsync(path);
            }

            var content = response.EnsureSuccessStatusCode().Content;

            var responseBody = await content.ReadAsStringAsync();

            return responseBody.Deserialize<T>();
        }
    }
}
