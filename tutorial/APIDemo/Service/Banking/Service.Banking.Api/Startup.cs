using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Policy;
using Common.Support.ErrorHandling;
using EventBus.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Banking.ApiImp;
using Service.Banking.Contract.Service;


namespace Service.Banking.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        readonly string apiVersion = "1"; 
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // ****STD***Register the Swagger generator, defining 1 or more Swagger documents  
            services.AddChtSwagger(apiVersion, "Banking API example");
            IoC.DI.Banking.DIContainer.Instance.ResgisterServices(services, Configuration);
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

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            //var svcHandler = new QuListener("rabbitmq");
            //var qSvc = new PaymentService();
            Action subscribeAct = () =>
            {
                var svcHandler = app.ApplicationServices.GetRequiredService<QuListener>();
                var qSvc = app.ApplicationServices.GetRequiredService<IPaymentService>();
                svcHandler.Subscribe<IPaymentService>(qSvc);
            };
            RetryHelper.AutoRetry(subscribeAct, 3);
        }
    }
}
