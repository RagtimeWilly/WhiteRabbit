using System.Collections.Generic;

namespace WhiteRabbit
{
    public class ExchangeConfig
    {
        public ExchangeConfig(string name, string exchangeType)
           : this(name, exchangeType, false, false, null)
        {
        }

        public ExchangeConfig(string name, string exchangeType, bool durable, 
            bool autoDelete, IDictionary<string, object> arguments)
        {
            Name = name;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;
            Arguments = arguments;
        }

        public string Name { get; }

        public string ExchangeType { get; }

        public bool Durable { get; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Arguments { get; set; }

        public bool IsDefaultExchange => string.IsNullOrEmpty(Name);

        public static ExchangeConfig Default()
        {
            return new ExchangeConfig(string.Empty, "direct");
        }
    }
}
