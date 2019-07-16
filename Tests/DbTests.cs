using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PressMachineServices.Press;
using Tests.Base;

namespace Tests
{
    [TestClass]
    public class DbTests : TestBase
    {
        [TestMethod]
        public void ConnectToDb()
        {
            using (var scope = this.CreateContainer())
            {
                IPressService pressService = scope.Resolve<IPressService>();


                var dfdf = pressService.GetPressOperationData(null);
            }
        }
    }
}
