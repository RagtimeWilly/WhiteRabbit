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

        public static void DeclareAndBindQueue(this IModel channel, QueueConfig queue)
        {
            if (!queue.Exchange.IsDefaultExchange)
            {
                channel.ExchangeDeclare(queue.Exchange.Name, queue.Exchange.ExchangeType, queue.Exchange.Durable,
                    queue.Exchange.AutoDelete, queue.Exchange.Arguments);
            }

            channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Args);

            if (!queue.Exchange.IsDefaultExchange)
            {
                channel.QueueBind(queue.Name, queue.Exchange.Name, queue.RoutingKey, queue.BindingArgs);
            }
        }
    }
}
