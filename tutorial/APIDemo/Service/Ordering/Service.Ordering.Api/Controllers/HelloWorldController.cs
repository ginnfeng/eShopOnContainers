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
    //[Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    public class HelloWorldController : ControllerBase, IHelloWorldService
    {
       
        private readonly ILogger<HelloWorldController> _logger;

        public HelloWorldController(ILogger<HelloWorldController> logger)
        {
            _logger = logger;
        }


        //[HttpGet]        
        [ApiSpec(HTTP.GET,typeof(IHelloWorldService), nameof(IHelloWorldService.DefaultGet))]        
        public IEnumerable<HelloWeather> DefaultGet()
        {
            return svc.DefaultGet();
        }
        //[HttpPost]
        //[Route("Hello")]
        [ApiSpec(HTTP.POST,typeof(IHelloWorldService), nameof(IHelloWorldService.Hello))]
        
        public HelloWeather Hello(string id1,[FromQuery]long id2, [FromHeader]DateTime id3, [FromBody]HelloInput inp)
        {
            return svc.Hello(id1,id2,id3,inp);
        }
       
        [ApiSpec(HTTP.GET, typeof(IHelloWorldService), nameof(IHelloWorldService.HelloGet))]
        public string HelloGet(string id1, string id2)//[QuieryBody]
        {
            return svc.HelloGet(id1,id2);
        }
        [ApiSpec(HTTP.POST, typeof(IHelloWorldService), nameof(IHelloWorldService.HelloPost))]
        public string HelloPost([FromForm]string id1, [FromForm]string id2)// [FromBody]不能超過一個且不可與[FromForm]混用，[FromForm]以key-value放在request body
        {
            return svc.HelloPost(id1,id2);
        }
        [ApiSpec(HTTP.POST, typeof(IHelloWorldService), nameof(IHelloWorldService.OneWayCall))]
        public void OneWayCall(string id1, [FromBody]  HelloInput inp)
        {
            svc.OneWayCall(id1, inp);
        }

        IHelloWorldService svc = new HelloWorldService();
    }

    

}
