using RabbitMQ.Client;

namespace WhiteRabbit
{
    public static class IModelEx
    {
        public static void DeclareExchange(this IModel channel, ExchangeConfig exchange)
        {
            if (!string.IsNullOrEmpty(exchange.Name))
            { 
                channel.ExchangeDeclare(exchange.Name, exchange.ExchangeType, exchange.Durable, 
                    exchange.AutoDelete, exchange.Arguments);
            }
        }
    }
}
