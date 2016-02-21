using System.Collections.Generic;

namespace WhiteRabbit
{
    public class TopologyBootstrapper
    {
        private readonly IModelFactory _modelFactory;
        private readonly IEnumerable<ExchangeConfig> _exchanges;
        private readonly IEnumerable<QueueConfig> _queueConfigs;

        public TopologyBootstrapper(
            IModelFactory modelFactory, 
            IEnumerable<ExchangeConfig> exchanges, 
            IEnumerable<QueueConfig> queueConfigs)
        {
            _modelFactory = modelFactory;
            _exchanges = exchanges;
            _queueConfigs = queueConfigs;
        }

        public void Init()
        {
            var channel = _modelFactory.CreateModel();

            foreach (var e in _exchanges)
            {
                channel.DeclareExchange(e);
            }

            foreach (var q in _queueConfigs)
            {
                channel.DeclareAndBindQueue(q);
            }

            channel.Dispose();
        }
    }
}
