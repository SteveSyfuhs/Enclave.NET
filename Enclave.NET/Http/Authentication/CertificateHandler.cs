using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System;
using System.Security.Cryptography.X509Certificates;
using Enclave.NET.Configuration;
using System.Net.Security;
using System.Collections.Generic;

namespace Enclave.NET.Http.Authentication
{
    public class CertificateHandler : AuthenticationHandler<CertificateAuthenticationOptions>
    {
        public CertificateHandler(IOptionsMonitor<CertificateAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            ClaimsIdentity identity = null;

            try
            {
                identity = Options.CertificateValidator(Context.Connection.ClientCertificate);
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex));
            }

            if (identity == null || !identity.IsAuthenticated)
            {
                return Task.FromResult(AuthenticateResult.Fail("Certificate could not be authenticated"));
            }

            var cert = Context.Connection.ClientCertificate;

            var result = AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), "certificate"));

            return Task.FromResult(result);
        }

        public static bool ValidateCertificate(X509Certificate2 certificate, X509Chain chain, SslPolicyErrors errors, IEnumerable<Certificate> clientCertificates)
        {
            foreach (var client in clientCertificates)
            {
                if (client.Matches(certificate))
                {
                    return ValidateCertificate(certificate, client);
                }
            }

            return false;
        }

        private static bool ValidateCertificate(X509Certificate2 certificate, Certificate client)
        {
            if (client.VerifyChain)
            {
                try
                {
                    certificate.Verify();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var now = DateTimeOffset.UtcNow;

            if (certificate.NotBefore > now || certificate.NotAfter < now)
            {
                return false;
            }

            return true;
        }
    }
}
