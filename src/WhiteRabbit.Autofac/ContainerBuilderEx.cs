using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using WhiteRabbit.Autofac.Scoping;

namespace WhiteRabbit.Autofac
{
    public static class ContainerBuilderEx
    {
        public static void RegisterSerializersFrom(this ContainerBuilder builder, IEnumerable<string> assemblyNames)
        {
            var assemblies = assemblyNames.Select(GetAssemblyFor);

            builder.RegisterSerializersFrom(assemblies);
        }

        public static void RegisterSerializersFrom(this ContainerBuilder builder, IEnumerable<Assembly> assemblies)
        {
            foreach (var a in assemblies)
            {
                builder
                    .RegisterAssemblyTypes(a)
                    .Where(t => typeof (ISerializer).IsAssignableFrom(t))
                    .As<ISerializer>()
                    .InstancePerLifetimeScope();
            }
        }

        public static void RegisterHandlersFrom<THandler>(this ContainerBuilder builder, IEnumerable<string> assemblyNames)
        {
            var assemblies = assemblyNames.Select(GetAssemblyFor);

            builder.RegisterHandlersFrom<THandler>(assemblies);
        }

        public static void RegisterHandlersFrom<THandler>(this ContainerBuilder builder, IEnumerable<Assembly> assemblies)
        {
            foreach (var a in assemblies)
            {
                builder
                    .RegisterAssemblyTypes(a)
                    .Where(t => typeof(THandler).IsAssignableFrom(t))
                    .As<THandler>()
                    .InstancePerLifetimeScope();
            }
        }

        public static ScopedHandlerChain RegisterScopedHandlerChain(this ContainerBuilder builder)
        {
            return new ScopedHandlerChain(builder);
        }

        public static HandlerChain RegisterHandlerChain(this ContainerBuilder builder)
        {
            return new HandlerChain(builder);
        }

        public static void RegisterHandlerChainAs(this ContainerBuilder builder, params Type[] handlers)
        {
            if (!handlers.All(h => h.GetInterfaces().Contains(typeof(IMessageHandler))))
            {
                throw new InvalidOperationException($"Every handler in the chain must implement '{nameof(IMessageHandler)}'");
            }

            var chain = new HandlerChain(builder);

            chain.StartWith(handlers.First());

            foreach (var h in handlers.Skip(1))
            {
                chain.Then(h);
            }
        }

        internal static void RegisterHandlerWithInner<TOuter, TInnerHandler>(this ContainerBuilder builder, bool firstInChain, string registrationName)
        {
            var currentType = typeof(TOuter);
            var nextType = typeof(TInnerHandler);

            var innerHandlerParameter = currentType
                .GetConstructors()
                .SelectMany(c => c.GetParameters())
                .FirstOrDefault(p => p.ParameterType == typeof(IMessageHandler));

            if (innerHandlerParameter == null)
            {
                throw new InvalidOperationException(
                    $"Unable to register '{nextType.Name}' as inner handler of '{nextType.Name}'. " +
                    $"No constructor parameter of type '{nameof(IMessageHandler)}' found in '{nextType.Name}'");
            }

            builder
                .RegisterType(nextType)
                .As<TInnerHandler>();

            if (firstInChain)
            {
                builder
                    .RegisterType(currentType)
                    .WithParameter(
                        new ResolvedParameter(
                            (p, c) => p.Name == innerHandlerParameter.Name,
                            (p, c) => c.Resolve<TInnerHandler>()))
                    .Named<TOuter>(registrationName)
                    .As<IMessageHandler>();
            }
            else
            {
                builder
                    .RegisterType(currentType)
                    .WithParameter(
                        new ResolvedParameter(
                            (p, c) => p.Name == innerHandlerParameter.Name,
                            (p, c) => c.Resolve<TInnerHandler>()))
                    .Named<TOuter>(registrationName)
                    .As<TOuter>();
            }
        }

        private static Assembly GetAssemblyFor(string s)
        {
            var assembly = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SingleOrDefault(x => x.GetName().Name == s);

            return assembly ?? Assembly.Load(s);
        }
    }
}