using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Threading.Tasks;

namespace EnclaveAzureKeyVaultServiceHostSample
{
    internal static class KeyVault
    {
        public static IConfigurationRoot Configuration { get; } = new ConfigurationBuilder()
                                                                              .SetBasePath(Directory.GetCurrentDirectory())
                                                                              .AddJsonFile("settings.json")
                                                                              .AddEnvironmentVariables().Build();

        public static KeyVaultConfiguration ServiceConfiguration => Configuration.GetSection("KeyVault").Get<KeyVaultConfiguration>();

        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var config = ServiceConfiguration;

            var clientCredential = new ClientCredential(config.ClientId, config.ClientSecret);

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }

        public static KeyVaultClient CreateClient()
        {
            var config = ServiceConfiguration;

            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
        }
    }
}
