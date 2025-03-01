using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Test
{
    [TestFixture]
    public class BootStrapperTests
    {
        [Test]
        public void Boot_Validate()
        {
            var container = BootStrapper.Boot();
            Assert.IsNotNull(container);
        }
    }
}
