using NUnit.Framework;
using ServiceStack;
using ServiceStack.Testing;
using Reusable.Rest.Implementations.SS;
namespace MyApp.Tests
{
    public class UnitTest
    {
        private readonly ServiceStackHost appHost;
        public UnitTest()
        {
            appHost = new BasicAppHost().Init();
            appHost.Container.AddTransient<PingService>();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();
        [Test]
        public void Can_call_MyServices()
        {
            var service = appHost.Container.Resolve<PingService>();
            var response = (string)service.Any(new Ping());
            Assert.That(response, Is.EqualTo("System up and running!"));
        }
    }
}
