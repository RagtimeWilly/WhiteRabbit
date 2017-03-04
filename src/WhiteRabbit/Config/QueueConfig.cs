using System;
using System.Collections.Generic;

namespace WhiteRabbit
{
    public class QueueConfig
    {
        public QueueConfig(string name)
            : this(name, new RoutingConfig(string.Empty))
        {
        }

        public QueueConfig(string name, RoutingConfig routingConfig)
            : this(name, routingConfig, ExchangeConfig.Default)
        {
        }

        public QueueConfig(string name, RoutingConfig routingConfig, ExchangeConfig exchange)
            : this(name, routingConfig, exchange, true, false, false, null)
        {
        }

        public QueueConfig(string name, RoutingConfig routingConfig, ExchangeConfig exchange, bool durable, bool exclusive, 
            bool autoDelete, IDictionary<string, object> args)
        {
            Exchange = exchange;
            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Args = args;
            RoutingConfig = routingConfig;
        }

        public ExchangeConfig Exchange { get; set; }

        public string Name { get; set; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Args { get; set; }

        public RoutingConfig RoutingConfig { get; set; }

        public QueueConfig WithTTL(TimeSpan ttl)
        {
            if (Args == null)
            {
                Args = new Dictionary<string, object>();
            }

            Args.Add("x-message-ttl", Convert.ToInt32(ttl.TotalMilliseconds));

            return this;
        }
    }
}
