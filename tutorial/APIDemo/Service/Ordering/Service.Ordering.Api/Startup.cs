using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Common.Policy;
using Microsoft.OpenApi;
using EventBus.RabbitMQ;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Servic;

namespace Service.Ordering.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        readonly string apiVersion ="1";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // ****STD***Register the Swagger generator, defining 1 or more Swagger documents            
            services.AddChtSwagger(apiVersion,"Ordering API example");
            IoC.DI.Ordering.DIContainer.ResgisterServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // ****STD*** Enable middleware to serve generated Swagger as a JSON endpoint.           
            app.UseChtSwagger(apiVersion);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // ****STD***
            ConfigureEventBus(app);
        }

        // ****STD***
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            //var svcHandler = new QuListener("rabbitmq");
            //var qSvc = new PaymentCallbackService();
            var svcHandler = app.ApplicationServices.GetRequiredService<QuListener>();            
            var qSvc = app.ApplicationServices.GetRequiredService<IPaymentCallbackService>();
            svcHandler.Subscribe<IPaymentCallbackService>(qSvc);
        }
    }
}
