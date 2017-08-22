using System;
using System.Security.Cryptography.X509Certificates;

namespace Enclave.NET.Configuration
{
    public class Certificate
    {
        public string Thumbprint { get; set; }

        public string PublicKey { get; set; }

        public bool VerifyChain { get; set; }

        public bool Matches(X509Certificate2 certificate)
        {
            if (!string.IsNullOrWhiteSpace(PublicKey))
            {
                return MatchesPublicKey(certificate);
            }

            if (!string.IsNullOrWhiteSpace(Thumbprint))
            {
                return MatchesThumbprint(certificate);
            }

            return false;
        }

        private bool MatchesThumbprint(X509Certificate2 certificate)
        {
            var expectedThumbprint = Thumbprint.Replace(" ", "").ToLowerInvariant();

            var actualThumbprint = certificate.Thumbprint.Replace(" ", "").ToLowerInvariant();

            return string.Equals(expectedThumbprint, actualThumbprint);
        }

        private bool MatchesPublicKey(X509Certificate2 certificate)
        {
            var actualPublicKey = certificate.GetPublicKey();

            var expectedPublicKey = Convert.FromBase64String(PublicKey);

            return ConstantTimeEquals(actualPublicKey, expectedPublicKey);
        }

        private static bool ConstantTimeEquals(byte[] left, byte[] right)
        {
            var diff = left.Length ^ right.Length;

            for (int i = 0; i < left.Length && i < right.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
        }
    }
}