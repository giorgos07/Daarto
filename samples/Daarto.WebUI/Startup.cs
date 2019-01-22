using System;
using Daarto.Abstractions;
using Daarto.Infrastructure.Identity;
using Daarto.Infrastructure.Settings;
using Daarto.Models;
using Daarto.Services;
using Identity.Dapper.Postgres;
using Identity.Dapper.Postgres.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;

namespace Daarto.WebUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment) {
            var builder = new ConfigurationBuilder().SetBasePath(hostingEnvironment.ContentRootPath)
                                                    .AddJsonFile("appsettings.json", true, true)
                                                    .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container. For more information on how to configure your 
        // application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            // Get the connection string from appsettings.json file.
            var connectionString = Configuration.GetConnectionString("DbConnection");

            // Add and configure the default identity system that will be used in the application.
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddUserManager<UserManager<ApplicationUser>>()
                    .AddRoleManager<RoleManager<ApplicationRole>>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddDapperStores(connectionString)
                    .AddDefaultTokenProviders();

            // Add support for non-distributed memory cache in the application.
            services.AddMemoryCache();

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.Configure<IdentityOptions>(options => {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 6;
                options.Lockout.AllowedForNewUsers = true;
                // User settings.
                options.User.RequireUniqueEmail = true;
            });

            // Configure cookie settings.
            services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/log-off";
                options.AccessDeniedPath = "/account/login";
                options.SlidingExpiration = true;
            });

            // Map appsettings.json file elements to a strongly typed class.
            services.Configure<AppSettings>(Configuration);
            // Add services required for using options.
            services.AddOptions();

            // Configure custom services to be used by the framework.
            services.AddTransient<IDatabaseConnectionFactory>(e => new SqlConnectionFactory(connectionString));
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.AddSingleton<ICacheManagerService, CacheManagerService>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IEmailService>(e => new EmailService(new SmtpSettings {
                From = Configuration["SmtpSettings:From"],
                Host = Configuration["SmtpSettings:Host"],
                Port = int.Parse(Configuration["SmtpSettings:Port"]),
                SenderName = Configuration["SmtpSettings:SenderName"],
                LocalDomain = Configuration["SmtpSettings:LocalDomain"],
                Password = Configuration["SmtpSettings:Password"],
                UserName = Configuration["SmtpSettings:UserName"]
            }));

            // Add and configure MVC services.
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddJsonOptions(setupAction => {
                        // Configure the contract resolver that is used when serializing .NET objects to JSON and vice versa.
                        setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory) {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (hostingEnvironment.IsDevelopment()) {
                applicationBuilder.UseDeveloperExceptionPage();
            } else {
                applicationBuilder.UseExceptionHandler("/error/index/500");
            }

            applicationBuilder.UseStaticFiles(new StaticFileOptions {
                OnPrepareResponse = staticFileResponseContext => {
                    // Configure caching for static files. Files will be cached for 365 days and duration must be provided in seconds.
                    const int maxAge = 365 * 24 * 3600;
                    staticFileResponseContext.Context.Response.Headers[HeaderNames.CacheControl] = $"public,max-age={maxAge}";
                }
            });

            applicationBuilder.UseAuthentication();
            applicationBuilder.UseStatusCodePagesWithRedirects("/error/index?errorCode={0}");

            applicationBuilder.UseMvc(routes => {
                routes.MapRoute("administrationAreaRoute", "{area:exists}/{controller=home}/{action=index}/{id?}");
                routes.MapRoute("defaultRoute", "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}