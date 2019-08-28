using DbToRest.Core.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace DbToRest.Tests
{
    [TestFixture]
    public class BaseTest
    {
        public IHostingEnvironment iHostingEnvironment { get; set; }
        public IServiceCollection iServiceCollection { get; set; }
        public IConfiguration configuration;
        public IApplicationBuilder applicationBuilder { get; set; }

        private Mock<IHostingEnvironment> mockHostingEnvironment;
        private Mock<IApplicationBuilder> mockApplicationBuilder;

        public BaseTest()
        {
            mockHostingEnvironment = new Mock<IHostingEnvironment>();
            mockHostingEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mockHostingEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            mockHostingEnvironment.Setup(x => x.EnvironmentName).Returns("Staging");

            //mockHostingEnvironment.(x => x.IsDevelopment()).Returns(true);
            mockApplicationBuilder = new Mock<IApplicationBuilder>();

            var host = new WebHostBuilder()
            .UseStartup<Startup>()
            .UseEnvironment("IntegrationTest");


            this.configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();


            iHostingEnvironment = mockHostingEnvironment.Object;
            iServiceCollection = new ServiceCollection();

            iServiceCollection.Add(new ServiceDescriptor(typeof(IHostingEnvironment), iHostingEnvironment));

            iServiceCollection.ConfigureApplicationServices(this.configuration, iHostingEnvironment);

            mockApplicationBuilder.Object.ConfigureRequestPipeline();


            
        }
    }
}