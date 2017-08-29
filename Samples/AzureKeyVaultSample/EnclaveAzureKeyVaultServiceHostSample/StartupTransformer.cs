using Enclave.NET;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EnclaveAzureKeyVaultServiceHostSample
{
    public class StartupTransformer : IStartupTransform
    {
        public void Transform(IServiceCollection services)
        {
            services.AddScoped<IKeyStorageService, KeyVaultStorageService>();
            services.AddScoped<ICryptoProcessor, KeyVaultCryptoProcessor>();
        }
    }
}
