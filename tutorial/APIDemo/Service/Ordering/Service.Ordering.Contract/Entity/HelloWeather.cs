////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 5/6/2020 5:16:14 PM 
// Description: HelloWeather.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Ordering.Contract.Entity
{
    public class HelloWeather
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
