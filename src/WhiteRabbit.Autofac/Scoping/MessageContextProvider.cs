using System;

namespace WhiteRabbit.Autofac.Scoping
{
    internal class MessageContextProvider
    {
        private IMessageContext _context;

        public void SetContext(IMessageContext context)
        {
            _context = context;
        }

        public IMessageContext Get()
        {
            if (_context == null)
                throw new Exception("Message context has not been set");

            return _context;
        }
    }
}
