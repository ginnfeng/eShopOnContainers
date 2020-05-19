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
    
    //[Route("api/[controller]")]
    public class HelloWorldController : ControllerBase, IHelloWorldService
    {
       
        private readonly ILogger<HelloWorldController> _logger;

        public HelloWorldController(ILogger<HelloWorldController> logger)
        {
            _logger = logger;
        }


        //[HttpGet]        
        [ApiSpec(HTTP.GET,typeof(IHelloWorldService), "DefaultGet")]        
        public IEnumerable<HelloWeather> DefaultGet()
        {
            return svc.DefaultGet();
        }
        //[HttpPost]
        //[Route("Hello")]
        [ApiSpec(HTTP.POST,typeof(IHelloWorldService), "Hello")]
        
        public HelloWeather Hello(string id1,[FromQuery]int id2, [FromHeader]DateTime id3, [FromBody]HelloInput inp)
        {
            return svc.Hello(id1,id2,id3,inp);
        }


        IHelloWorldService svc = new HelloWorldService();
    }

    

}
