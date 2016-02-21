using System;
using System.Collections.Generic;
using WhiteRabbit.SampleApp.Interfaces;

namespace WhiteRabbit.SampleApp.Infrastructure.Messaging
{
    public class CommandDispatcher : DispatcherBase<ICommandHandler>
    {
        public CommandDispatcher(IEnumerable<ICommandHandler> handlers) 
            : base(typeof(ICommandHandler<>), handlers)
        {
        }

        public override void Dispatch<TCommand>(TCommand cmd)
        {
            if (Handlers.ContainsKey(cmd.GetType()))
            {
                dynamic handler = Handlers[cmd.GetType()];

                handler.Handle((dynamic)cmd);
            }
            else
            {
                throw new Exception("No command handler found for command:" + cmd.GetType());
            }
        }
    }
}
