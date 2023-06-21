using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Start;

namespace CRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("App Startup");
            CreateHostBuilder(args).Build().Run();
            Console.WriteLine("Database Connection Stopped");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
