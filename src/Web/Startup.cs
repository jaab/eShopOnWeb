﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Globalization;

using Ardalis.ListStartupServices;
using Microsoft.eShopWeb.ApplicationCore;
using Infrastructure.Services;
using Infrastructure.Services.CurrencyService;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.eShopWeb.Infrastructure.Logging;
using Microsoft.eShopWeb.Web.Interfaces;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using Web.Extensions;
using Web.Extensions.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.eShopWeb.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

[assembly : ApiConventionType(typeof(DefaultApiConventions))]
[assembly : RootNamespace("Microsoft.eShopWeb.Web")]
namespace Microsoft.eShopWeb.Web {
    public class Startup {
        private IServiceCollection _services;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureDevelopmentServices(IServiceCollection services) {
            services.AddSingleton<ICurrencyService, CurrencyServiceStatic>();

            // use in-memory database
            ConfigureInMemoryDatabases(services);
            // use real database
        }

        private void ConfigureInMemoryDatabases(IServiceCollection services) {
            // use in-memory database
            services.AddDbContext<CatalogContext>(c =>
                c.UseInMemoryDatabase("Catalog"));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));

            ConfigureServices(services);
        }

        public void ConfigureProductionServices(IServiceCollection services) {
            services.AddSingleton<ICurrencyService, CurrencyServiceExternal>();
            // use real database
            // Requires LocalDB which can be installed with SQL Server Express 2016
            // https://www.microsoft.com/en-us/download/details.aspx?id=54284
            services.AddDbContext<CatalogContext>(dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString("CatalogConnection")));

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            ConfigureServices(services);
        }
        public void ConfigureAzureServices(IServiceCollection services) {
            ConfigureProductionServices(services);
        }

        public void ConfigureTestingServices(IServiceCollection services) {
            ConfigureInMemoryDatabases(services);
        }

        private static void CreateIdentityIfNotCreated(IServiceCollection services) {
            var sp = services.BuildServiceProvider();
            using(var scope = sp.CreateScope()) {
                var existingUserManager = scope.ServiceProvider
                    .GetService<UserManager<ApplicationUser>>();
                if (existingUserManager == null) {
                    services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddDefaultUI()
                        .AddEntityFrameworkStores<AppIdentityDbContext>()
                        .AddDefaultTokenProviders();
                }
            }
        }

        private static void ConfigureCookieSettings(IServiceCollection services) {
            services.Configure<CookiePolicyOptions>(options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.Cookie = new CookieBuilder {
                    IsEssential = true // required for auth to work without explicit user consent; adjust to suit your privacy policy
                };
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services) {
            ConfigureCookieSettings(services);

            CreateIdentityIfNotCreated(services);

            services.AddMediatR(typeof(BasketViewModelService).Assembly);
            services.AddMediatR(typeof(WishViewModelService).Assembly);

            if (_webHostEnvironment.IsDevelopment()) {
                services.AddSingleton<ICurrencyService, CurrencyServiceStatic>();
            } else {
                services.AddSingleton<ICurrencyService, CurrencyServiceExternal>();
            }

            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddCatalogServices(Configuration);
            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
            
            services.AddMvc();
            services.AddOptions();
            services.Configure<EmailConfig>(Configuration.GetSection("EmailSender"));
            services.AddTransient<IEmailSender, EmailSender>();
            
            
           services.AddAuthentication()
            .AddFacebook(options =>
            {
                options.AppId = "183476352866825";
                options.AppSecret = "471535fb6e0542b83029d5b24ae3b3c5";

            });

             services.AddMvc(option => option.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            #region snippet1
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            #endregion
             services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("fr")
                    };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
    
            // Add memory cache services
            services.AddMemoryCache();

            services.AddRouting(options => {
                // Replace the type and the name used to refer to it with your own
                // IOutboundParameterTransformer implementation
                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });

            services.AddMvc(options => {
                options.Conventions.Add(new RouteTokenTransformerConvention(
                    new SlugifyParameterTransformer()));

            });
            services.AddRazorPages(options => {
                options.Conventions.AuthorizePage("/Basket/Checkout");
            });
            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApi.Models.OpenApiInfo { Title = "My API", Version = "v1" }));

            services.AddHealthChecks();

            services.Configure<ServiceConfig>(config => {
                config.Services = new List<ServiceDescriptor>(services);

                config.Path = "/allservices";
            });

            _services = services; // used to debug registered services
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            #region snippet2
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("fr"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            #endregion
            app.UseBenchmarking();
            app.UseHealthChecks("/health",
                new HealthCheckOptions {
                    ResponseWriter = async(context, report) => {
                        var result = JsonConvert.SerializeObject(
                            new {
                                status = report.Status.ToString(),
                                    errors = report.Entries.Select(e => new {
                                        key = e.Key,
                                            value = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                                    })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
            if (env.IsDevelopment()) {
                // app.UseDeveloperExceptionPage();
                app.UseShowAllServicesMiddleware();
                app.UseDatabaseErrorPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseMvcWithDefaultRoute();
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute("default", "{controller:slugify=Home}/{action:slugify=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("home_page_health_check");
                endpoints.MapHealthChecks("api_health_check");
            });

        }
    }
}
