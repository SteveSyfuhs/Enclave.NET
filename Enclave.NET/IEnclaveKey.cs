using System;
using System.Collections.Generic;
using System.Text;

namespace Enclave.NET
{
    public interface IEnclaveKey
    {
        string KeyType { get; }

        object RetrieveKey();

        T RetrieveKey<T>();
    }
}
