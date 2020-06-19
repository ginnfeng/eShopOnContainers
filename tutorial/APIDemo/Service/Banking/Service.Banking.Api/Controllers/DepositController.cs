using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Banking.ApiImp;
using Service.Banking.Contract.Service;
using Sid.Bss.Banking;

namespace Service.Banking.Api.Controllers
{
    [ApiSpec(typeof(IDepositService))]
    [ApiController]
    public class DepositController : ControllerBase, IDepositService
    {
        
        // GET: api/Banking
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "Deposit API" };
        }

        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.WireDepositForPayment))]
        public bool WireDepositForPayment(string accountId, [FromBody] PaymentDetail detail)
        {
            return svc.WireDepositForPayment(accountId, detail);
        }
        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.CreateBankAccount))]
        public BankAccount CreateBankAccount(string cid)
        {
            return svc.CreateBankAccount(cid);
        }

        IDepositService svc = new PaymentService();
    }
        
}
