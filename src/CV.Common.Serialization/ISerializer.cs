using System;

namespace CV.Common.Serialization
{
    public interface ISerializer
    {
        string Serialize(object obj);

        string Serialize<TType>(TType obj);

        TType Deserialize<TType>(string serialized);

        object Deserialize(Type type, string serialized);
    }
}
