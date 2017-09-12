using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CRED2
{
    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) => { })
            .UseStartup<Startup>()
            .Build();

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
    }
}