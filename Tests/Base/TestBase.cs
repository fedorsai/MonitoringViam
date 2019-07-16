using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using PressMachineServices.Infrastructure;

namespace Tests.Base
{
    public class TestBase
    {
        /// <summary>
        ///     Вернет контейнер.
        /// </summary>
        /// <returns>IContainer</returns>
        /// <returns>Autofac.IContainer</returns>
        protected virtual IContainer CreateContainer()
        {
             var builder = new ContainerBuilder();

            builder.RegisterModule(new AutofacConfigModule());

            var container = builder.Build();

            return container;
        }
    }
}
