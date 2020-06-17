using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Banking.Contract.Entity;
using Service.Banking.Contract.Service;

namespace Service.Banking.Api.Controllers
{
    [ApiSpec(typeof(IPaymentService))]
    [ApiController]
    public class PaymentController : ControllerBase, IPaymentService
    {
        
        // GET: api/Banking
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "Payment API" };
        }


        [ApiSpec(HTTP.POST, typeof(IPaymentService), nameof(IPaymentService.BankTransfers))]
        public QuResult<TransferRecord> BankTransfers(string fromId, string toId, [FromBody]PaymentDetail detail)
        {
            throw new NotImplementedException();
        }
        [ApiSpec(HTTP.POST, typeof(IPaymentService), nameof(IPaymentService.CardTransfer))]
        public QuResult<TransferRecord> CardTransfer(string fromId, string toId, [FromBody] PaymentDetail detail)
        {
            throw new NotImplementedException();
        }
        [ApiSpec(HTTP.POST, typeof(IPaymentService), nameof(IPaymentService.WireTransfer))]
        public void WireTransfer(string toId, [FromBody] PaymentDetail detail)
        {
            throw new NotImplementedException();
        }
        [ApiSpec(HTTP.POST, typeof(IPaymentService), nameof(IPaymentService.WireDeposit))]
        public bool WireDeposit(string accountId, [FromBody] PaymentDetail detail)
        {
            throw new NotImplementedException();
        }
    }
        
}
