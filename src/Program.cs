using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Conduit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls($"http://localshot:5000")
                .UseStartup<Startup>()
                //.UseDefaultServiceProvider(options =>
                //                                options.ValidateScopes = false)
            ;
    }
}
