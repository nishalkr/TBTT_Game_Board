using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TBTT_Data.Interface;
using TBTT_Data.Infrastructure;
using AutoMapper;
using TBTT_Board.Infrastructure;
using Serilog;
using TBTT_Logging;
using TBTT_Board.ActionFilter;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;

namespace TBTT_Board
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(c => c.AddProfile<AutoMapping>(), typeof(Startup));
            services.Add(new ServiceDescriptor(typeof(IAvailableRepository), typeof(AvailableRepository), ServiceLifetime.Transient)); // Transient
            services.Add(new ServiceDescriptor(typeof(IWaitingRepository), typeof(WaitingRepository), ServiceLifetime.Transient)); // Transient
            services.Add(new ServiceDescriptor(typeof(IGameRepository), typeof(GameRepository), ServiceLifetime.Transient)); // Transient
            services.Add(new ServiceDescriptor(typeof(ICourtRepository), typeof(CourtRepository), ServiceLifetime.Transient)); // Transient
            services.Add(new ServiceDescriptor(typeof(IMemberRepository), typeof(MemberRepository), ServiceLifetime.Transient)); // Transient
            services.AddControllersWithViews();
            //services.AddMvc(options =>
            //{
            //    options.Filters.Add(new ShortCircuitingResourceFilter());
            //});
            //services.AddSingleton<IConfiguration>(Configuration);

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
            app.UseSerilogRequestLogging();

            app.UseRouting();
                       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller}/{action}/{id?}",
                 defaults: new { controller = "Home", action = "Index" });

                //endpoints.MapControllerRoute(
                //     name: "defaultWithoutAction",
                //     pattern: "{controller}/{id?}",
                //     defaults: new { action = "Display" }
                //   );

                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
