using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave.NET
{
    public interface IStartupTransform
    {
        void Transform(IServiceCollection services);
    }
}
