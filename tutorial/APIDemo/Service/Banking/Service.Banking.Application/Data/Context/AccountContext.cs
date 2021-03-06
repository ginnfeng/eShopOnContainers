﻿////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 6/17/2020 11:06:19 AM 
// Description: AccountContext.cs  
// Revisions  :            		
// **************************************************************************** 

using Sid.Bss.Banking;
using Common.Support;

using System.Collections.Generic;


namespace Service.Banking.Application.Data.Context
{

   
    public class AccountContext
    {   //暫時模擬DB data
        static public AccountContext Instance => Singleton<AccountContext>.Instance;
        public AccountContext()
        {
            accountRepository = new Dictionary<string, BankAccount>()
            { //模擬客戶銀行帳戶，C表消客，B表企客
                {"C0001",new BankAccount{ Id="C0001",AccountBalance=1000} },
                {"C0002",new BankAccount{ Id="C0002",AccountBalance=2000} },
                {"C0003",new BankAccount{ Id="C0003",AccountBalance=3000} },
                {"C0004",new BankAccount{ Id="C0004",AccountBalance=4000} },
                {"B0001",new BankAccount{ Id="B0001",AccountBalance=10000} }
            };
        }
        public bool TryGetValue(string id, out BankAccount account)
        {
            lock (this)
            {
                return accountRepository.TryGetValue(id, out account);
            }
        }
        public bool Insert(BankAccount account)
        {
            lock (this)
            {
                if (accountRepository.ContainsKey(account.Id))
                    return false;
                accountRepository[account.Id] = account;
                return true;
            }
        }
        public bool Update(BankAccount account)
        {
            lock (this)
            {
                if (!accountRepository.ContainsKey(account.Id))
                    return false;
                accountRepository[account.Id] = account;
                return true;
            }
        }
        
        private Dictionary<string, BankAccount> accountRepository = new Dictionary<string, BankAccount>();
    }
}
