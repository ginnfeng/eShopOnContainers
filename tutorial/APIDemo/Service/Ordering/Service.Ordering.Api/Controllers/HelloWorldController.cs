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
    [ApiSpec(typeof(IHelloWorldService))]
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
        [ApiSpec(typeof(IHelloWorldService), "DefaultGet")]        
        public IEnumerable<HelloWeather> DefaultGet()
        {
            return svc.DefaultGet();
        }
        [ApiSpec(typeof(IHelloWorldService), "Hello")]
        public string Hello(string id)
        {
            return svc.Hello(id);
        }


        IHelloWorldService svc = new HelloWorldService();
    }

    

}
