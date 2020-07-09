////*************************Copyright © 2020 Feng 豐**************************	
// Created    : 7/8/2020 2:49:24 PM 
// Description: Test_Exception.cs  
// Revisions  :            		
// **************************************************************************** 
using Support.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Text;
using UTDll;
namespace UTool.Test
{
    class Test_Exception : UTest
    {
        public Test_Exception()
        {
            //
            // TODO: Add constructor logic here
            //      
        }
        
        [UMethod]
        public void T_Retry()
        {// TODO: Add Testing logic here
            var xo = new OutService();
            var ts=new TimeSpan(0, 0, 3);//Retry 間隔時間
            int retryNum = 2;//Retry 之次數限制
            try
            {
                RetryHelper.AutoRetry(() => xo.CallWebServiceFunction(), ts, retryNum);
            }
            catch (Exception e)
            {
                //Here! To Log Error!
                throw e;
            }
            
        }
    }
    class OutService
    {
        public void CallWebServiceFunction()
        {
            //......呼叫介面，其可能throw Exception;
        }
    }
}
