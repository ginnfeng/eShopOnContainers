using ApiGw.ClientProxy;
using ApiGw.ClientProxy.Ext;
using Common.DataContract;
using EventBus.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.HelloWorld.Contract.Servic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class WeatherForecastService
    {
        private IConnSource<IApiSetting> _src;
        private ILoggerFactory _loggerFactory;
        public WeatherForecastService(IConnSource<IApiSetting> src, ILoggerFactory loggerFactory)
        {
            _src = src;
            _loggerFactory = loggerFactory;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
        async public Task<string> Hello()
        {
            var proxy = new ApiProxy<IHelloWorldService>(_src, _loggerFactory);
            proxy.ApiVersion = "1";// 
            //proxy.RegisterSwaggerDoc(new Uri(swaggerDocUrl));
            proxy.RegisterChtSwaggerDoc(useApiGateway: true);
            return await Task.Run<string>(() => proxy.Svc.HelloGet("EEE", "FFF"));            
        }
        public WeatherForecast Data { get { return new WeatherForecast(); } }
    }
}
