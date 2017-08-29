using Enclave.NET.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace EnclaveAzureKeyVaultServiceHostSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Enclave Service Host...");

            EnclaveServiceHost.BuildWebHost(args).Run();
        }
    }
}
