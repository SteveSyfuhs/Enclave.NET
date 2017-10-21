using Enclave.NET.Configuration;
using Enclave.NET.Core;
using Enclave.NET.Http.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Enclave.NET.Http
{
    public static class EnclaveServiceHost
    {
        public static IWebHost BuildWebHost<T>(string[] args)
            where T : class, new()
        {
            return CreateBuilder<T>(args).Build();
        }

        public static IWebHostBuilder CreateBuilder<T>(string[] args)
            where T : class, new()
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<T>()
                          .UseKestrel(options =>
                          {
                              var server = ServiceStartup.Configuration.GetSection("Server").Get<Server>();

                              options.Listen(IPAddress.Loopback, server.Port, listenOptions =>
                              {
                                  var certificate = FindServerCertificate(server);

                                  listenOptions.UseHttps(new HttpsConnectionAdapterOptions()
                                  {
                                      ServerCertificate = certificate,
                                      ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                                      SslProtocols = SslProtocols.Tls12,
                                      ClientCertificateValidation = (a, b, c) => CertificateHandler.ValidateCertificate(a, b, c, server.ClientCertificates)
                                  });
                              });
                          });
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return BuildWebHost<ServiceStartup>(args);
        }

        private static X509Certificate2 FindServerCertificate(Server server)
        {
            var store = new X509Store(server.ServerCertificate.StoreName, server.ServerCertificate.StoreLocation);

            store.Open(OpenFlags.ReadOnly);

            try
            {
                var locals = store.Certificates.Find(X509FindType.FindByThumbprint, server.ServerCertificate.Thumbprint, false);

                if (locals.Count == 1)
                {
                    return locals[0];
                }
            }
            finally
            {
                store.Close();
            }

            return null;
        }

        private class ServiceStartup
        {
            private static IConfigurationRoot _Configuration;

            public static IConfigurationRoot Configuration => _Configuration ??
                    (_Configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("settings.json")
                            .AddJsonFile("settings.local.json", optional: true)
                            .AddEnvironmentVariables().Build());

            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.UseMvc();
                app.UseDeveloperExceptionPage();
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvcCore(config =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .AddAuthenticationSchemes(CertificateAuthenticationOptions.Scheme)
                                    .Build();

                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonFormatters()
                .AddAuthorization();

                services.AddScoped<ICryptoProcessor, InMemoryCryptoProcessor>();
                services.AddScoped<ICryptoProcessor, InMemoryCryptoProcessor>();
                services.AddScoped<IKeyStorageService, InMemoryStorageService>();
                services.AddScoped<IEnclaveService, EnclaveCryptoService>();
                services.AddScoped<ITokenAuthenticator, KerberosTokenAuthenticator>();
                services.AddScoped<ISchemeAuthenticator, SchemeAuthenticator>();

                services.AddTransient<IDistributedCache>(c => null);

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CertificateAuthenticationOptions.Scheme;
                    options.DefaultChallengeScheme = CertificateAuthenticationOptions.Scheme;

                })
                .AddScheme<CertificateAuthenticationOptions, CertificateHandler>(
                    CertificateAuthenticationOptions.Scheme,
                    c => { }
                );

                ExternalTransform(services);
            }

            private void ExternalTransform(IServiceCollection services)
            {
                var startupTransformType = Configuration.GetValue<string>("StartupTransformType");

                if (string.IsNullOrWhiteSpace(startupTransformType))
                {
                    return;
                }

                var transformType = Type.GetType(startupTransformType);

                if (transformType != null)
                {
                    var transform = (IStartupTransform)Activator.CreateInstance(transformType);

                    if (transform != null)
                    {
                        transform.Transform(services);
                    }
                }
            }
        }
    }
}
