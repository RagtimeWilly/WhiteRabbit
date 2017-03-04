using System;
using RabbitMQ.Client;

namespace WhiteRabbit
{
    public static class IModelEx
    {
        public static void DeclareExchange(this IModel channel, ExchangeConfig exchange)
        {
            if (!exchange.IsDefaultExchange)
            { 
                channel.ExchangeDeclare(exchange.Name, exchange.ExchangeType, exchange.Durable, 
                    exchange.AutoDelete, exchange.Arguments);
            }
        }

        public static void DeclareAndBindQueue(this IModel channel, QueueConfig queue)
        {
            channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Args);

            if (!queue.Exchange.IsDefaultExchange)
            {
                channel.DeclareExchange(queue.Exchange);

                foreach (var key in queue.RoutingConfig.RoutingKeys)
                {
                    channel.QueueBind(queue.Name, queue.Exchange.Name, key, queue.RoutingConfig.BindingArgs);
                }
            }
        }
    }
}
