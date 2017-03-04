using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;

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
