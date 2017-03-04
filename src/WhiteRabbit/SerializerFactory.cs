using System;
using System.Collections.Generic;
using System.Linq;

namespace WhiteRabbit
{
    public class SerializerFactory : ISerializerFactory
    {
        private readonly IDictionary<string, ISerializer> _serializers;

        public SerializerFactory(IEnumerable<ISerializer> serializers)
        {
            _serializers = serializers.ToDictionary(s => s.ContentType, s=> s);
        }

        public ISerializer For(string contentType)
        {
            if (!_serializers.ContainsKey(contentType))
                throw new Exception($"No know serializer for content type: {contentType}");

            return _serializers[contentType];
        }
    }
}
