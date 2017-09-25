using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tools.Connector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Web.Data;
using Shop.Web.Models;
using Shop.Web.Services;

namespace Shop.Web
{

    

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                                        options.UseSqlServer(Configuration.GetConnectionString("ShopIdentity")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            services.AddMvc(options =>
                            {
                                options.SslPort = 44321;
                                options.Filters.Add(new RequireHttpsAttribute());
                            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }
        // This method gets called by the runtime. Use this method to configure container.

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var node = ConnectToNode();
            builder.RegisterInstance(node).As<IGridDomainNode>();
            builder.RegisterInstance(node).As<ICommandExecutor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        class ShopNodeAddress : IAkkaNetworkAddress
        {
            public string SystemName { get; } = "shop_console";
            public string Host { get; } = "localhost";
            public string PublicHost { get; } = "localhost";
            public int PortNumber { get; } = 5001;
            public bool EnforceIpVersion { get; } = true;
        }
        private IGridDomainNode ConnectToNode()
        {
            var connector = new GridNodeConnector(new ShopNodeAddress());
            connector.Connect().Wait();
            return connector;
        }
    }
}
