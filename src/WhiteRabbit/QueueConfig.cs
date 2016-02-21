using System.Collections.Generic;

namespace WhiteRabbit
{
    public class QueueConfig
    {
        public QueueConfig(string name)
            : this(name, string.Empty)
        {
        }

        public QueueConfig(string name, string routingKey)
            : this(name, routingKey, ExchangeConfig.Default())
        {
        }

        public QueueConfig(string name, string routingKey, ExchangeConfig exchange)
            : this(name, routingKey, exchange, true, false, false, null, null)
        {
        }

        public QueueConfig(string name, string routingKey, ExchangeConfig exchange, bool durable, bool exclusive, 
            bool autoDelete, IDictionary<string, object> args, IDictionary<string, object> bindingArgs)
        {
            Exchange = exchange;
            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Args = args;
            RoutingKey = routingKey;
            BindingArgs = bindingArgs;
        }

        public ExchangeConfig Exchange { get; set; }

        public string Name { get; set; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Args { get; set; }

        public string RoutingKey { get; set; }

        public IDictionary<string, object> BindingArgs { get; set; }
    }
}
