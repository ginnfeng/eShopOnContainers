using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Support.Thread;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Ordering.Contract;
using Service.Ordering.Contract.Entity;
using Service.Ordering.ImpApi;

namespace Service.Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        [HttpPost]
        [Route("IssueOrder")]
        public async Task<IActionResult> IssueOrder(Order order)
        {
            var wrapper = new AsyncCallWrapper<IOrderingService>(svc);
            await wrapper.AsyncCall(itSvc => itSvc.IssueOrder(order));
            return Ok();
        }
        [HttpGet]
        [Route("QueryOrder")]
        public async Task<ActionResult<Order>> QueryOrder(string orderId)
        {
            //Order order = new Order();
            var wrapper = new AsyncCallWrapper<IOrderingService>(svc);
            Order order = await wrapper.AsyncCall(itSvc => itSvc.QueryOrder(orderId));
            return Ok(order);
        }
        private IOrderingService svc = new OrderingService();
        //// GET: api/Ordering
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Ordering/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Ordering
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/Ordering/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
