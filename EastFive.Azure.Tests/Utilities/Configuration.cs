using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;


namespace EastFive.Azure.Tests.Utilities
{
    public static class Configuration
    {
        public static void Construct()
        {
            var path = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
               .SetBasePath(path)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            EastFive.Web.Configuration.ConfigurationExtensions.Initialize(configuration);
        }
    }
}
