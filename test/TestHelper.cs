using System.IO;
using MTQueue.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace UnitTests
{
    public class TestHelper
    {
        public static IOptions<AppSettings> getConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json");
            IConfigurationRoot configuration = builder.Build();
            AppSettings config = new AppSettings();
            configuration.Bind(config);
            return Options.Create(config);
        }
    }
}