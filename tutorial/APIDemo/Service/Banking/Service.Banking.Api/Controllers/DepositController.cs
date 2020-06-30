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
    public class DepositController : ControllerBase//, IDepositService
    {
        public DepositController(IDepositService svc)
        {
            this.svc = svc;
        }
        // GET: api/Banking
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "Deposit API" };

        }
        [ApiSpec(HTTP.PUT, typeof(IDepositService), nameof(IDepositService.WireDepositForPayment))]
        public async Task<ActionResult<bool>> WireDepositForPayment(string accountId, [FromBody] PaymentDetail detail)
        {
            var rlt = await Task.Run<bool>(() => svc.WireDepositForPayment(accountId, detail));
            return Ok(rlt);
        }

        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.CreateBankAccount))]
        public async Task<ActionResult<BankAccount>> CreateBankAccount(string cid)
        {
            var rlt = await Task.Run<BankAccount>(() => svc.CreateBankAccount(cid));
            return Ok(rlt);
        }
        //public BankAccount CreateBankAccount(string cid)
        //{
        //    return svc.CreateBankAccount(cid);
        //}

        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.Deposit))]
        public async Task<ActionResult<BankAccount>> Deposit(string accountId, decimal amount)
        {
            var rlt = await Task.Run<BankAccount>(() => svc.Deposit(accountId, amount));
            return Ok(rlt);
        }

        private IDepositService svc;//= new PaymentService();
    }
        
}
