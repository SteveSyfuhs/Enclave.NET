using Enclave.NET.Http;
using System;
using Microsoft.AspNetCore.Hosting;

namespace Enclave.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            EnclaveServiceHost.BuildWebHost(args).Run();
        }
    }
}
