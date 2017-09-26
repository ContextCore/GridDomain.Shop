using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Event;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace Shop.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var log = InitLogger();
            try
            {
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return 0;
        }

        private static ILogger InitLogger()
        {
            return Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                                         .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                                         .Enrich.FromLogContext()
                                                         .WriteTo.RollingFile(".\\Logs\\log_{HalfHour}.txt")
                                                         .WriteTo.Console()
                                                         .CreateLogger();
        }

        public static IWebHost BuildWebHost(string[] args) =>
                                       WebHost.CreateDefaultBuilder(args)
                                              .ConfigureServices(s => s.AddAutofac())
                                              .UseStartup<Startup>()
                                              .UseKestrel()
                                              .UseSerilog()
                                              .Build();
    }
}