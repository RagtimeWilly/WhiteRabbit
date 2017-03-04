using System;

namespace WhiteRabbit
{
    public interface ISerializer
    {
        string ContentType { get; }

        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] bytes);

        object DeserializeObject(byte[] bytes, Type type);
    }
}
