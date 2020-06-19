using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Ordering.ApiImp;
using Service.Ordering.Contract.Service;
using Sid.Bss.Ordering;
using Support.ThreadExt;

namespace Service.Ordering.Api.Controllers
{
    [ApiSpec(typeof(IOrderingService))]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        [ApiSpec(HTTP.POST, typeof(IOrderingService), nameof(IOrderingService.IssueOrder))]
        public async Task<IActionResult> IssueOrder([FromBody] Order order)
        {
            //var wrapper = new AsyncCallWrapper<IOrderingService>(svc);
            //await wrapper.AsyncCall(itSvc => itSvc.IssueOrder(order));
            await Task.Run(() => svc.IssueOrder(order));
            return Ok();
        }
        [ApiSpec(HTTP.GET, typeof(IOrderingService), nameof(IOrderingService.QueryOrder))]
        public async Task<ActionResult<Order>> QueryOrder(string orderId)
        {
            Order order = await Task.Run<Order>(() => svc.QueryOrder(orderId));
            return Ok(order);
        }

        [ApiSpec(HTTP.GET, typeof(IOrderingService), "")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "Ordering API" };
        }
        private IOrderingService svc= new OrderingService();
    }
}
