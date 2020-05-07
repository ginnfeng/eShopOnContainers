using System;
using System.Threading;
using UTDll;
using System.Xml;

namespace UTool.Test
{
	/// <summary>
	/// Summary description for Test_Demo.
	/// </summary>
	
	public class Test_Demo:UTest
	{
		public Test_Demo()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		[UMethod]
		public void T_StrLength(string inp1, string inp2)
		{
			printf("inp1={0} inp2={1}", inp1, inp2);
			assert(inp1.Length == inp2.Length);
			Thread.Sleep(1000);
			print("*******Test End*************");
		}

		[UMethod]
		public void T_IpLogParser(string ipLog)
		{// TODO: Add Testing logic here
			var ipParser = new IpLogParser();
			var ipList = ipParser.Parse(ipLog);
			ipList.ForEach(ip=>print(ip.ToString()));
			
		}

		[UMethod]
		public void T2()
		{
			messageBox("hello");
			Exception e=new Exception("ERROR!");
			throw e;
		}
		[UMethod]
		public void T3(string s)
		{		
			print("inp",parameters);
			print("loop",this.loopCounter);
			
		}
        [UMethod]
        public void T4(string s)
        {
            throw new Exception("Error");
        }
		[UMethod]
		public void T5(string s)
		{
			this.assert(s.Length > 3);

		}

	
	}


	
}


