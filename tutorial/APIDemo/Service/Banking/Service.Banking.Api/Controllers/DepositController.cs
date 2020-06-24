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
        public bool WireDepositForPayment(string accountId, [FromBody] PaymentDetail detail)
        {
            bool rlt= svc.WireDepositForPayment(accountId, detail);
            return rlt;
        }
        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.CreateBankAccount))]
        public BankAccount CreateBankAccount(string cid)
        {
            return svc.CreateBankAccount(cid);
        }

        [ApiSpec(HTTP.POST, typeof(IDepositService), nameof(IDepositService.Deposit))]
        public BankAccount Deposit(string accountId, decimal amount)
        {
            return svc.Deposit(accountId, amount);
        }

        IDepositService svc;//= new PaymentService();
    }
        
}
