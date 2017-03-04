using System.Collections.Generic;

namespace WhiteRabbit
{
    public class RoutingConfig
    {
        public RoutingConfig(string routingKey)
           : this(new List<string> { routingKey })
        {
        }

        public RoutingConfig(ICollection<string> routingKeys)
           : this(routingKeys, null)
        {
        }

        public RoutingConfig(string routingKey, IDictionary<string, object> bindingArgs)
            : this(new List<string> { routingKey }, bindingArgs)
        {
        }

        public RoutingConfig(ICollection<string> routingKeys, IDictionary<string, object> bindingArgs)
        {
            BindingArgs = bindingArgs;
            RoutingKeys = routingKeys;
        }

        public ICollection<string> RoutingKeys { get; set; }

        public IDictionary<string, object> BindingArgs { get; set; }
    }
}
