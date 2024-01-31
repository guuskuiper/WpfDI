using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Simple.WPF.DI.Tests
{
    public class WpfAppBuilderTests
    {
        private readonly WpfApp app;

        public WpfAppBuilderTests()
        {
            var builder = WpfApp.CreateBuilder();

            app = builder.Build();
        }

        [Fact]
        public void CreateIHostEnvironment()
        {
            IHostEnvironment hostEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

            // The ApplicationName is not equal to "Simple.WPF.DI.Tests", but is created by the testing framework
            Assert.False(string.IsNullOrEmpty(hostEnvironment.ApplicationName)); 

            Assert.Equal(Directory.GetCurrentDirectory(), hostEnvironment.ContentRootPath.TrimEnd('\\')); 
        }
        
        [Fact]
        public void AppEnvironment()
        {
            IHostEnvironment hostEnvironment = app.Environment;

            // The ApplicationName is not equal to "Simple.WPF.DI.Tests", but is created by the testing framework
            Assert.False(string.IsNullOrEmpty(hostEnvironment.ApplicationName)); 
        }

        [Fact]
        public void CreateLoggerFactory()
        {
            ILoggerFactory? loggerFactory = app.Services.GetService<ILoggerFactory>();

            Assert.NotNull(loggerFactory);
        }

        [Fact]
        public void CreateILogger()
        {
            ILogger<WpfAppBuilderTests>? logger = app.Services.GetService<ILogger<WpfAppBuilderTests>>();

            Assert.NotNull(logger);
        }
    }
}