using NLog.Web;
using System.Security.Cryptography;

namespace Task_Manager
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            GenerateKey();

            CreateHostBuilder(args, config).Build().Run();
        }

        private static string GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var keyBytes = new byte[32]; 
                rng.GetBytes(keyBytes);
                var base64Key = Convert.ToBase64String(keyBytes);

                // Вывод ключа в консоль
                Console.WriteLine("Generated Key: " + base64Key);

                return base64Key;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.AddConfiguration(config.GetSection("ConnectionStrings"));
                });
    }
}

