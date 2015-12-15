using System;
using Jil;

namespace CV.Common.Serialization.Json
{
    public class JsonSerializer : ISerializer
    {
        private readonly Options _options;

        public JsonSerializer(Options options)
        {
            _options = options;
        }

        public string Serialize(object obj)
        {
            return JSON.Serialize(obj, _options);
        }

        public string Serialize<TType>(TType obj)
        {
            return JSON.Serialize(obj, _options);
        }

        public TType Deserialize<TType>(string serialized)
        {
            return JSON.Deserialize<TType>(serialized, _options);
        }

        public object Deserialize(Type type, string serialized)
        {
            return JSON.Deserialize(serialized, type, _options);
        }
    }
}
