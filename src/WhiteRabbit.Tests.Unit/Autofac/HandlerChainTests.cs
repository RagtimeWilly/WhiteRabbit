using System;
using System.Text;
using Autofac;
using NUnit.Framework;
using RabbitMQ.Client.Events;
using WhiteRabbit.Autofac;

namespace WhiteRabbit.Tests.Unit.Autofac
{
    [TestFixture]
    public class HandlerChainTests
    {
        private TestContext _context;
        
        [SetUp]
        public void Setup()
        {
            _context = new TestContext();
        }

        [Test]
        public void RegisterHandlerChainAs_registers_inner_handlers_in_order_abc()
        {
            _context.ArrangeABC();

            _context.Act();

            _context.AssertABC();
        }

        [Test]
        public void RegisterHandlerChainAs_registers_inner_handlers_in_order_bad()
        {
            _context.ArrangeBD();

            _context.Act();

            _context.AssertBD();
        }

        [Test]
        public void RegisterHandlerChainAs_throws_exception_if_handlers_do_not_implement_IMessageHandler()
        {
            var ex = Assert.Throws<ArgumentException>(() => _context.ArrangeInvalidHandlers());

            Assert.AreEqual("Handler String cannot be part of chain as it does not implement 'IMessageHandler'", ex.Message);
        }

        private class TestContext
        {
            private readonly StringBuilder _messageSequence;
            private readonly ContainerBuilder _builder;
            private IContainer _container;

            public TestContext()
            {
                _builder = new ContainerBuilder();

                _messageSequence = new StringBuilder();

                _builder
                    .Register(c => _messageSequence)
                    .As<StringBuilder>();
            }

            public void ArrangeABC()
            {
                _builder
                    .RegisterHandlerChain()
                    .StartWith<MessageHandlerA>()
                    .Then<MessageHandlerB>()
                    .Then<MessageHandlerC>();

                _container = _builder.Build();
            }

            public void ArrangeBD()
            {
                _builder
                    .RegisterHandlerChain()
                    .StartWith<MessageHandlerB>()
                    .Then<MessageHandlerD>();

                _container = _builder.Build();
            }

            public void ArrangeInvalidHandlers()
            {
                _builder
                    .RegisterHandlerChain()
                    .StartWith<MessageHandlerB>()
                    .Then<string>();

                _container = _builder.Build();
            }

            public void Act()
            {
                using (var scope = _container.BeginLifetimeScope())
                {
                    var handler = scope.Resolve<IMessageHandler>();

                    handler.Handle(new BasicDeliverEventArgs());
                }
            }

            public void AssertABC()
            {
                var result = _messageSequence.ToString();

                Assert.AreEqual("~A~~B~~C~", result);
            }

            public void AssertBD()
            {
                var result = _messageSequence.ToString();

                Assert.AreEqual("~B~~D~", result);
            }

            private class MessageHandlerA : IMessageHandler
            {
                private readonly IMessageHandler _innerHandler;
                private readonly StringBuilder _builder;

                public MessageHandlerA(IMessageHandler innerHandler, StringBuilder builder)
                {
                    _innerHandler = innerHandler;
                    _builder = builder;
                }

                public void Handle<T>(T args)
                {
                    _builder.Append("~A~");

                    var msg = new MessageB { TextB = "~B~" };

                    _innerHandler.Handle(msg);
                }
            }

            private class MessageHandlerB : IMessageHandler
            {
                private readonly IMessageHandler _innerHandler;
                private readonly StringBuilder _builder;

                public MessageHandlerB(IMessageHandler innerHandler, StringBuilder builder)
                {
                    _innerHandler = innerHandler;
                    _builder = builder;
                }

                public void Handle<T>(T args)
                {
                    var msgB = args as MessageB;

                    if (msgB == null)
                        msgB = new MessageB { TextB = "~B~" };

                    _builder.Append(msgB.TextB);

                    var msgC = new MessageC { TextC = "~C~" };

                    _innerHandler.Handle(msgC);
                }
            }

            private class MessageHandlerC : IMessageHandler
            {
                private readonly StringBuilder _builder;

                public MessageHandlerC(StringBuilder builder)
                {
                    _builder = builder;
                }

                public void Handle<T>(T args)
                {
                    var msgC = args as MessageC;

                    _builder.Append(msgC.TextC);
                }
            }
            
            private class MessageHandlerD : IMessageHandler
            {
                private readonly StringBuilder _builder;

                public MessageHandlerD(StringBuilder builder)
                {
                    _builder = builder;
                }

                public void Handle<T>(T args)
                {
                    _builder.Append("~D~");
                }
            }

            private class MessageB
            {
                public string TextB { get; set; }
            }

            private class MessageC
            {
                public string TextC { get; set; }
            }
        }
    }
}
