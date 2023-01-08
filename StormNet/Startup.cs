using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StormNet
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            services.AddSingleton<DataHandler>();
            services.AddSingleton<DataProxy>();
            services.AddSingleton<DataOrchestrator>();
            services.AddSignalR();

            // Add ability to get any service lazy
            services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));
        }
        
        internal class Lazier<T> : Lazy<T> where T : class
        {
            public Lazier(IServiceProvider provider)
                : base(provider.GetRequiredService<T>)
            {
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {          
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRHub>("/signalR");
            });
        }
    }
}