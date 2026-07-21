using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Telinha.Core.Factory;
using Telinha.Core.Services;


namespace Telinha.Forms.Container
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.Register(c => new MemoryCache(new MemoryCacheOptions()))
              .As<IMemoryCache>().SingleInstance();
            builder.RegisterType<AppConfigServices>().AsSelf().SingleInstance();

            builder.RegisterType<ApiClientFactory>().AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<ApiClientFactory>().GetTMDB())
              .AsSelf()
              .SingleInstance();

            builder.Register(c => c.Resolve<ApiClientFactory>().GetDeepL())
              .AsSelf()
              .InstancePerDependency();

            builder.RegisterType<MidiaServices>().AsSelf();
            builder.RegisterType<Token>().AsSelf();
            builder.RegisterType<Home>().AsSelf();

            return builder.Build();
        }
    }
}