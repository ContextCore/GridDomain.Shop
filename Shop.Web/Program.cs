using System;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Shop.Web.Identity;
using Swashbuckle.AspNetCore.Swagger;
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
                                              .ConfigureServices(s =>
                                                                 {
                                                                     s.AddAutofac();
                                                                     s.AddMvc();
                                                                     s.AddSwaggerGen(c =>
                                                                                     {
                                                                                         c.SwaggerDoc("v1", new Info { Title = "Shop API", Version = "v1" });
                                                                                   });
                                                                     s.AddIdentity<AppUser, IdentityRole>
                                                                             (o =>
                                                                              {
                                                                                  // configure identity options
                                                                                  o.Password.RequireDigit = false;
                                                                                  o.Password.RequireLowercase = false;
                                                                                  o.Password.RequireUppercase = false;
                                                                                  o.Password.RequireNonAlphanumeric = false;
                                                                                  o.Password.RequiredLength = 6;
                                                                              })
                                                                             .AddEntityFrameworkStores<ShopIdentityDbContext>()
                                                                             .AddDefaultTokenProviders();

                                                                     s.AddAutoMapper();
                                                                 })
                                              .UseStartup<Startup>()
                                              .UseKestrel()
                                              .UseSerilog()
                                              .Build();
    }
}