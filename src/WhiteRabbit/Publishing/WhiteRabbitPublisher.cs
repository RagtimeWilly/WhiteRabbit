using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public sealed class WhiteRabbitPublisher : PublisherBase, IPublisher
    {
        public WhiteRabbitPublisher(IModelFactory modelFactory, ISerializerFactory serializerFactory, string exchangeName)
            : base(modelFactory, serializerFactory, exchangeName)
        {
        }

        public async Task Publish<T>(T msg, string routingKey, Guid correlationId, string contentType)
        {
            await Task.Run(() =>
            {
                base.PublishMessage(msg, routingKey, correlationId, contentType);
            });
        }
    }
}
