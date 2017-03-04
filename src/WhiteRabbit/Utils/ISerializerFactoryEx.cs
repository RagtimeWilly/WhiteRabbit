using System;
using RabbitMQ.Client.Events;

namespace WhiteRabbit
{
    internal static class ISerializerFactoryEx
    {
        public static byte[] Serialize(this ISerializerFactory serializerFactory, object o, string contentType)
        {
            var serializer = serializerFactory.For(contentType);

            return serializer.Serialize(o);
        }

        public static TResult DeserializeTo<TResult>(this ISerializerFactory serializerFactory, BasicDeliverEventArgs args)
            where TResult : class
        {
            if (string.IsNullOrEmpty(args.BasicProperties?.ContentType))
            {
                throw new Exception("No content type set on message:" + args);
            }

            var contentType = args.BasicProperties.ContentType;

            var serializer = serializerFactory.For(contentType);

            return serializer.Deserialize<TResult>(args.Body);
        }

        public static object DeserializeToType(this ISerializerFactory serializerFactory, BasicDeliverEventArgs args, Type t)
        {
            if (string.IsNullOrEmpty(args.BasicProperties?.ContentType))
            {
                throw new Exception("No content type set on message:" + args);
            }

            var contentType = args.BasicProperties.ContentType;

            var serializer = serializerFactory.For(contentType);

            return serializer.DeserializeObject(args.Body, t);
        }
    }
}
