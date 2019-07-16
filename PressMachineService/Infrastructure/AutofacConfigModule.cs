using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PressMachineServices.Database;
using PressMachineServices.Opc;
using PressMachineServices.Press;

namespace PressMachineServices.Infrastructure
{
    public class AutofacConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new PressMachineDbContex()).As<PressMachineDbContex>().InstancePerLifetimeScope();

            builder.RegisterType<PressService>().As<IPressService>().InstancePerLifetimeScope();

            builder.RegisterType<OpcService>().As<IOpcService>().InstancePerLifetimeScope();
        }
    }
}
