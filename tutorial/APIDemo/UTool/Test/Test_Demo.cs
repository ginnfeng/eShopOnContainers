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
		public void T1(int inp)
		{			
			int i=100;
			string s=i.ToString();
			print("value",i);
			assert(i==inp);
			Thread.Sleep(1000);
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
