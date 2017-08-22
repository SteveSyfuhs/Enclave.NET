using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Enclave.NET.Http.Authentication
{
    public class CertificateAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "certificate";

        public CertificateAuthenticationOptions()
        {
            CertificateValidator = certificate =>
            {
                return new ClaimsIdentity(new List<Claim>() {
                    new Claim(ClaimTypes.Name, certificate.Subject)
                }, "certificate");
            };
        }

        public Func<X509Certificate2, ClaimsIdentity> CertificateValidator { get; set; }

        public override void Validate()
        {
            if (CertificateValidator == null)
            {
                throw new InvalidOperationException("CertificateValidator has not been set.");
            }

            base.Validate();
        }
    }
}
