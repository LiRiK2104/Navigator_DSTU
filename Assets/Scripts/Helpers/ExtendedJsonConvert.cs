using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Helpers
{
    public static class ExtendedJsonConvert
    {
        public static string SerializeMultitypes([CanBeNull] object value, params Type[] types)
        {
            return JsonConvert.SerializeObject(value, GetJsonSerializerSettings(types));
        }
        
        public static T DeserializeMultitypes<T>(string json, params Type[] types)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings(types));
        }

        private static JsonSerializerSettings GetJsonSerializerSettings(params Type[] types)
        {
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new KnownTypesBinder { KnownTypes = types.ToList() }
            };
        }
        
        private class KnownTypesBinder : ISerializationBinder
        {
            public IList<Type> KnownTypes { get; set; }

            public Type BindToType(string assemblyName, string typeName)
            {
                return KnownTypes.SingleOrDefault(t => t.Name == typeName);
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }
        }
    }
}
