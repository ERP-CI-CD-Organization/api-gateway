using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace APIGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;
                    var env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    webBuilder.UseKestrel();
                    // webBuilder.UseStartup<Startup>();
                    webBuilder.UseStartup(assemblyName);
                    webBuilder.ConfigureAppConfiguration(config => config.AddJsonFile($"ocelot.{ env }.json"));
                    webBuilder.UseUrls("http://+:8080");
                    
                }).ConfigureLogging(logging => logging.AddConsole());
    }
}
