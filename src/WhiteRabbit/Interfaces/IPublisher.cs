using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public interface IPublisher
    {
        Task Publish<T>(T msg, string routingKey, Guid correlationId, string contentType);
    }
}
