using System;
using System.Threading.Tasks;

namespace WhiteRabbit
{
    public interface IConsumer : IDisposable
    {
        Task Start(bool noAck);
    }
}
