using Funq;
using ServiceStack;
using NUnit.Framework;
using Reusable.Rest.Implementations.SS;
namespace MyApp.Tests
{
    public class IntegrationTest
    {
        const string BaseUri = "http://localhost:2000/";
        private readonly ServiceStackHost appHost;
        class AppHost : AppSelfHostBase
        {
            public AppHost() : base(nameof(IntegrationTest), typeof(PingService).Assembly)
            {
                Licensing.RegisterLicenseFromFile("~/../ss_license.txt".MapHostAbsolutePath());
            }
            public override void Configure(Container container)
            {
            }
        }
        public IntegrationTest()
        {
            appHost = new AppHost()
                .Init()
                .Start(BaseUri);
        }
        [OneTimeTearDown]
        public void OneTimeTearDown() => appHost.Dispose();
        public IServiceClient CreateClient() => new JsonServiceClient(BaseUri);
        [Test]
        public void Can_call_Hello_Service()
        {
            var client = CreateClient();
            var response = client.Get<string>(new Ping());
            Assert.That(response, Is.EqualTo("System up and running!"));
        }
    }
}