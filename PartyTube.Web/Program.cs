using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PartyTube.Web
{
    public class Program
    {
        private const string IsServiceArg = "--service";
        private static readonly string[] MyArgs = {IsServiceArg};
        private static Logger _logger;

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var pathToContentRoot = IsService(args)
                ? Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
                : Directory.GetCurrentDirectory();

            var webHostArgs = GetWebHostArgs(args);
            var webHostBuilder = WebHost.CreateDefaultBuilder(webHostArgs)
                                        .ConfigureLogging(logging =>
                                         {
                                             logging.ClearProviders();
                                             logging.SetMinimumLevel(LogLevel.Trace);
                                         })
                                        .UseNLog() // NLog: setup NLog for Dependency injection
                                        .UseContentRoot(pathToContentRoot)
                                        .ConfigureServices(services => services.AddAutofac())
                                        .UseStartup<Startup>();


            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env)) env = EnvironmentName.Production;

            var config = new ConfigurationBuilder()
                        .SetBasePath(pathToContentRoot)
                        .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"hosting.{env}.json", optional: true, reloadOnChange: true)
                        .Build();
            return webHostBuilder.UseConfiguration(config);
        }

        private static string[] GetWebHostArgs(string[] args)
        {
            return args.Except(MyArgs).ToArray();
        }

        private static bool IsService(string[] args)
        {
            return args.Contains(IsServiceArg);
        }


        public static void Main(string[] args)
        {
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                _logger.Debug("init main");

                var builder = CreateWebHostBuilder(args);
                var host = builder.Build();

                if (IsService(args))
                {
                    host.RunAsService();
                }
                else
                {
                    host.Run();
                }
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                _logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }
    }
}