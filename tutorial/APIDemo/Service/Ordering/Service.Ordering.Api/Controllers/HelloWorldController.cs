using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract;
using Service.Ordering.Contract.Entity;
using Service.Ordering.Contract.Servic;

namespace Service.Ordering.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[ApiConfig(typeof(IHelloWorldService))]
    public class HelloWorldController : ControllerBase, IHelloWorldService
    {
       
        private readonly ILogger<HelloWorldController> _logger;

        public HelloWorldController(ILogger<HelloWorldController> logger)
        {
            _logger = logger;
        }


        //[HttpGet]
        //[Route("Hello")]
        //[ApiConfig(typeof(IHelloWorldService), "Hello")]   
        [HttpConfig(typeof(IHelloWorldService), "Hello")]//1
        [RouteConfig(typeof(IHelloWorldService), "Hello")]//2
        public IEnumerable<HelloWeather> Hello()
        {
            return svc.Hello();
        }

        IHelloWorldService svc = new HelloWorldService();
    }

    

}
