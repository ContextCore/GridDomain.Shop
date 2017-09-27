using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shop.Web.Identity;
using Swashbuckle.AspNetCore.Swagger;

namespace Shop.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection s)
        {
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
            // api user claim policy
            s.AddAuthorization(options =>
                                      {
                                          options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
                                      });

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));


        //    s.Configure<JwtIssuerOptions>(options => jwtAppSettingOptions.Bind(options));
            s.Configure<JwtIssuerOptions>(options =>
                                                 {
                                                     options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                                                     options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                                                     options.SigningCredentials = new SigningCredentials(CompositionRoot.SigningKey, SecurityAlgorithms.HmacSha256);
                                                 });


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = CompositionRoot.SigningKey,

                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            s.AddAuthentication(options =>
                                {
                                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                })
             .AddJwtBearer(o =>
                           {
                               o.TokenValidationParameters = tokenValidationParameters;
                               o.Events = new JwtBearerEvents()
                                          {
                                              OnAuthenticationFailed = c =>
                                                                       {
                                                                           c.NoResult();

                                                                           c.Response.StatusCode = 500;
                                                                           c.Response.ContentType = "text/plain";
                                                                           if(Environment.IsDevelopment())
                                                                           {
                                                                               // Debug only, in production do not share exceptions with the remote host.
                                                                               return c.Response.WriteAsync(c.Exception.ToString());
                                                                           }
                                                                           return c.Response.WriteAsync("An error occurred processing your authentication.");
                                                                       }
                                          };
                           });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1"));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            CompositionRoot.Configure(builder, Configuration);
        }

        
       
    }
}
