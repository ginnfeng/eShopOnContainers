using Microsoft.Extensions.Configuration;
using System;

namespace BlazorApp.Data
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
        public string ApiGatewayConnection { get { return Configuration.GetValue<string>("cfg_ApiGatewayConnection"); } }
        public string EventBusConnection { get { return Configuration.GetValue<string>("cfg_EventBusConnection"); } }
        static public IConfiguration Configuration { get; set; }
    }
}
