using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Serialization;

namespace CRED2.Helpers
{
    public class KnownTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return this.KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }
    }
}