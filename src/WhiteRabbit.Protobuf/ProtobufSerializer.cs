using System;
using System.IO;

namespace WhiteRabbit.Protobuf
{
    public class ProtobufSerializer : ISerializer
    {
        public string ContentType => "application/protobuf";

        public byte[] Serialize<T>(T obj)
        {
            byte[] result;

            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
                result = stream.ToArray();
            }

            return result;
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }

        public object DeserializeObject(byte[] bytes, Type type)
        {
            if (type.AssemblyQualifiedName == null)
            {
                throw new Exception($"AssemblyQualifiedName not set for {type}");
            }

            var clrType = Type.GetType(type.AssemblyQualifiedName);

            if (clrType == null)
            {
                throw new Exception($"Unable to find type for {type.AssemblyQualifiedName}");
            }

            using (var stream = new MemoryStream(bytes))
            {
                var obj = ProtoBuf.Serializer.NonGeneric.Deserialize(clrType, stream);
                return Convert.ChangeType(obj, clrType);
            }
        }
    }
}
