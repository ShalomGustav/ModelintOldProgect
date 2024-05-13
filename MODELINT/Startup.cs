using CoreLibrary.Interfaces;
using CoreLibrary.Models;
using CoreLibrary.Services;
using CoreLibrary.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MODELINT.Servises;
using YcLibrary.Interfaces;
using YcLibrary.Models.Extentions;

namespace MODELINT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddTransient<IValuteService, ValuteService>();
            services.AddTransient<ILoggerService, LoggerService>();
            services.AddTransient<IAuthorizationMiddlewareService, AuthorizationMiddlewareService>();
            services.AddTransient<ValidationService>();
            services.AddTransient<IExternalValuteService, ExternalValuteService>();
            services.AddTransient<IYcTranslateService, YcTranslateService>();

            services.AddSingleton<AuthorizationService>();
            services.AddSingleton<YcCredentials>();
            services.AddSingleton<Client>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "rest",
                    pattern: "rest/index",
                    defaults: new {controller="Rest",action = "index"}
                    ); 



            });
        }
    }
}
