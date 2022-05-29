using Autofac;
using AutoMapper;
using Divergic.Configuration.Autofac;
using Insomnia.AvatarBot.API.Configurations.AutoMapper;
using Insomnia.AvatarBot.BI.Options;
using System;
using Insomnia.AvatarBot.BI.Interfaces;
using Insomnia.AvatarBot.BI.Services;

namespace Insomnia.AvatarBot.API.Configurations.Autofac
{
    public class ServiceModules : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

                builder.RegisterType<Commands>()
                    .As<ICommands>();

            builder.Register(context => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            }
            )).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var resolver = new EnvironmentJsonResolver<Config>("appsettings.json", $"appsettings.{env}.json");
            var module = new ConfigurationModule(resolver);

            builder.RegisterModule(module);
        }
    }
}
