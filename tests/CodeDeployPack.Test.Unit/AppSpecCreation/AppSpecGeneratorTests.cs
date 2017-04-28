using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeDeployPack.AppSpecCreation;
using Moq;
using NUnit.Framework;

namespace CodeDeployPack.Test.Unit.AppSpecCreation
{
    [TestFixture]
    public class AppSpecGeneratorTests
    {
        [Test]
        public void A()
        {
            var asg = new AppSpecGenerator(null, null);
            var packageContents = new Dictionary<string, string>();

            var spec = asg.CreateAppSpec(packageContents);

            Assert.That(spec, Is.Not.Null);
        }

        [Test]
        public void CreateAppSpec_DiscoversHooks()
        {
            var discoverHooks = new Mock<IDiscoverHooks>();
            var packageContents = new Dictionary<string, string> { { "srcFoo", "dstFoo" }, { "srcBar", "dstBar" } };
            var destinationFiles = packageContents.Values;

            discoverHooks
                .Setup(h => h.Discover(destinationFiles))
                .Returns(new Hooks())
                .Verifiable();

            var asg = new AppSpecGenerator(null, discoverHooks.Object);

            asg.CreateAppSpec(packageContents);

            discoverHooks.VerifyAll();
        }
    }
}
