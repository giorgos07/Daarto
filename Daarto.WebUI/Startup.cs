using Daarto.IdentityProvider.Entities;
using Daarto.IdentityProvider.Stores;
using Daarto.Services.Abstract;
using Daarto.Services.Concrete;
using Daarto.WebUI.Infrastructure.Identity;
using Daarto.WebUI.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using System;
using Daarto.DataAccess.Abstract;
using Daarto.DataAccess.Concrete;
using Daarto.Services.Models;
using Daarto.WebUI.Infrastructure.Services;

namespace Daarto.WebUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder().SetBasePath(hostingEnvironment.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", true, true)
                                                    .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container. For more information on how to configure your 
        // application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection servicesCollection)
        {
            // Add and configure the default identity system that will be used in the application.
            servicesCollection.AddIdentity<ApplicationUser, ApplicationRole>()
                              .AddUserManager<ApplicationUserManager>()
                              .AddRoleManager<ApplicationRoleManager>()
                              .AddSignInManager<ApplicationSignInManager>()
                              .AddDefaultTokenProviders();
            
            // Add support for non-distributed memory cache in the application.
            servicesCollection.AddMemoryCache();

            servicesCollection.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            servicesCollection.Configure<IdentityOptions>(configureOptions =>
            {
                // Password settings.
                configureOptions.Password.RequireDigit = true;
                configureOptions.Password.RequiredLength = 6;
                configureOptions.Password.RequireNonAlphanumeric = true;
                configureOptions.Password.RequireUppercase = false;
                configureOptions.Password.RequireLowercase = true;

                // Lockout settings.
                configureOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                configureOptions.Lockout.MaxFailedAccessAttempts = 6;
                configureOptions.Lockout.AllowedForNewUsers = true;

                // Cookies settings.
                configureOptions.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(365);
                configureOptions.Cookies.ApplicationCookie.LoginPath = "/account/login";
                configureOptions.Cookies.ApplicationCookie.LogoutPath = "/account/log-off";
                configureOptions.Cookies.ApplicationCookie.AccessDeniedPath = "/account/login";
                configureOptions.Cookies.ApplicationCookie.CookieName = "daarto-cookie";
                configureOptions.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
                configureOptions.Cookies.ApplicationCookie.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                configureOptions.Cookies.ApplicationCookie.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                // User settings.
                configureOptions.User.RequireUniqueEmail = true;
            });

            // Map appsettings.json file elements to a strongly typed class.
            servicesCollection.Configure<AppSettings>(Configuration);

            // Add services required for using options.
            servicesCollection.AddOptions();

            // Add and configure MVC services.
            servicesCollection.AddMvc()
                              .AddJsonOptions(setupAction =>
                              {
                                  // Configure the contract resolver that is used when serializing .NET objects to JSON and vice versa.
                                  setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                              });

            // Get the connection string from appsettings.json file.
            string connectionString = Configuration.GetConnectionString("DaartoDbConnection");

            // Configure custom services to be used by the framework.
            servicesCollection.AddTransient<IDatabaseConnectionService>(e => new DatabaseConnectionService(connectionString));
            servicesCollection.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            servicesCollection.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            servicesCollection.AddTransient<IEmailSender, MessageServices>();
            servicesCollection.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            servicesCollection.AddSingleton<ICacheManagerService, CacheManagerService>();
            servicesCollection.AddTransient<IUserRepository, UserRepository>();

            servicesCollection.AddTransient<IEmailService>(e => new EmailService(new SmtpSettings
            {
                From = Configuration["SmtpSettings:From"],
                Host = Configuration["SmtpSettings:Host"],
                Port = int.Parse(Configuration["SmtpSettings:Port"]),
                SenderName = Configuration["SmtpSettings:SenderName"],
                LocalDomain = Configuration["SmtpSettings:LocalDomain"],
                Password = Configuration["SmtpSettings:Password"],
                UserName = Configuration["SmtpSettings:UserName"]
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (hostingEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseBrowserLink();
            }
            else
            {
                applicationBuilder.UseExceptionHandler("/error/index/500");
            }

            applicationBuilder.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = staticFileResponseContext =>
                {
                    // Configure caching for static files. Files will be cached for 365 days and duration must be provided in seconds.
                    const int maxAge = 365 * 24 * 3600;
                    staticFileResponseContext.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={maxAge}";
                }
            });

            applicationBuilder.UseIdentity();
            applicationBuilder.UseStatusCodePagesWithRedirects("/error/index?errorCode={0}");

            applicationBuilder.UseMvc(routes =>
            {
                routes.MapRoute("administrationAreaRoute", "{area:exists}/{controller=home}/{action=index}/{id?}");
                routes.MapRoute("defaultRoute", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}