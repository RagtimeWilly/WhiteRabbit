using System;
using System.Text;
using Newtonsoft.Json;

namespace WhiteRabbit.Json
{
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);

            return Encoding.Default.GetBytes(json);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var json = Encoding.Default.GetString(bytes);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public object DeserializeObject(byte[] bytes, Type type)
        {
            var json = Encoding.Default.GetString(bytes);

            return JsonConvert.DeserializeObject(json, type);
        }
    }
}
