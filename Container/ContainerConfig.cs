using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Telinha.Factory;
using Telinha.Services;

namespace Telinha.Container
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            // Infra
            builder.Register(c => new MemoryCache(new MemoryCacheOptions()))
              .As<IMemoryCache>().SingleInstance();
            builder.RegisterType<FileCacheService>().AsSelf().SingleInstance();
            builder.RegisterType<AppConfigServices>().AsSelf().SingleInstance();

            // 🔥 ApiClientFactory cria tudo
            builder.RegisterType<ApiClientFactory>().AsSelf().SingleInstance();

            // 🔥 TMDBServices vem da factory, não do RegisterType
            builder.Register(c => c.Resolve<ApiClientFactory>().GetTMDB())
              .AsSelf()
              .SingleInstance();

            // 🔥 DEEPLContracts também se precisar
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