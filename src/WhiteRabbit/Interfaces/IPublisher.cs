using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public interface IPublisher<T> : IDisposable
    {
        Task Publish(T msg, string routingKey, Guid correlationId);
    }
}
