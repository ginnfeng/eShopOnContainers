using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
namespace ApiGw.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var apigwcfg = CfgGen.Instance.GenOcelotJsonFile();
            var builder=Host.CreateDefaultBuilder(args)
                //.ConfigureAppConfiguration(ic => ic.AddJsonFile(Path.Combine("configuration","configuration.json")))
                
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(ic => ic.AddJsonFile(apigwcfg));
            return builder;
        }
    }
}
