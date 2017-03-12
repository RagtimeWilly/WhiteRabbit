using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteRabbit.SampleApp.Infrastructure.Messaging
{
    internal class MessageCounter
    {
        private int _count = 0;

        public void Increment()
        {
            Interlocked.Increment(ref _count);
        }

        public int Count => _count;
    }
}
