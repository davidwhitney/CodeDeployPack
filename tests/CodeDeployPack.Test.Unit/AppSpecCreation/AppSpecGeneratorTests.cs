using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeDeployPack.AppSpecCreation;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit.AppSpecCreation
{
    [TestFixture]
    public class AppSpecGeneratorTests
    {
        [Test]
        public void A()
        {
            var asg = new AppSpecGenerator(null);
            var packageContents = new Dictionary<string, string>();

            var spec = asg.CreateAppSpec(packageContents);

            Assert.That(spec, Is.Not.Null);
        }
    }
}
