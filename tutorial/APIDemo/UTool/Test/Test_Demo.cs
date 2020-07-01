using System;
using System.Threading;
using UTDll;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Support.Net.Util;
using Common.Support.Common.DataCore;
using Support.Helper;
using System.Reflection;
using Common.DataContract;

namespace UTool.Test
{
	/// <summary>
	/// Summary description for Test_Demo.
	/// </summary>
	public class GG<T>
		//where T:class,new()
	{
		//public GG()
		//{
		//	Entity = new T();
		//}
		public GG(T t)
		{
			Entity = t;
		}
		public string GetMyType()
		{
			return Entity.GetType().ToString();
		}
		public T Entity { get; set; }
	}
	public class Test_Demo:UTest
	{
		public Test_Demo()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[UMethod]
		public void T_GG()
		{// TODO: Add Testing logic here
			var s = new GG<string>("1111");
			print(s.GetMyType());
			var i = new GG<int>(234);
			print(i.GetMyType());
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
			var a = new List<string>();
			var b = new List<int>();
			var ipParser = new IpLogParser();
			//var ipList = ipParser.Parse(ipLog);
			//ipList.ForEach(ip=>print(ip.ToString()));
			ipParser.ProcessEvent += IpParser_ProcessInternalEvent;
			ipParser.ProcessEvent += IpParser_ProcessExtEvent;
			ipParser.Processs(ipLog,null);
			Action<string, string> act = (s1, s2) => {  };
			Action<string, string> act1 = Act_1;
			Func<string, bool> fuc = Func_1;
			bool r=fuc("1111");
			bool r2 = Func_1("1111");
			//ipParser.Processs(ipLog, match=> { if (match.Groups[2].Length < 3) print(match.Groups[0]); });
			//ipParser.Processs(ipLog, match => { if (match.Groups[2].Length >= 3) print(match.Groups[0]); });
		}
		private bool Func_1(string s1)
		{
			return true;
		}
		private void Act_1(string s1, string s2)
		{
			//....
		}
		private void IpParser_ProcessInternalEvent(Match match)
		{
			if (match.Groups[2].Length < 3) print("INT:"+match.Groups[0]);
		}

		private void IpParser_ProcessExtEvent(Match match)
		{
			if (match.Groups[2].Length > 3) print("EXTT:"+match.Groups[0]);
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
		[UMethod]
		public void T6()
		{
			string s = "Provider=MSDataShape;Data Provider=SQLOLEDB;Data Source=(local);Initial Catalog=pubs;Integrated Security=SSPI;MaxConnection=10";
			//var rlt=s.ReadPairs('=', ';');
			var proxy=new ConnSourceProxy<IDbCfg>(s);
			var cfg = proxy.Entity;
			print($"Provider={cfg.Provider} DataSource={cfg.DataSource} MaxConnection={cfg.MaxConnection}");

			cfg = new DbCfg();
			proxy.ForEachSet(cfg);
			print($"Provider={cfg.Provider} DataSource={cfg.DataSource} MaxConnection={cfg.MaxConnection}");

		}

	}

	public interface IDbCfg
    {
        public string Provider { get; set; }
		[Indexing(Id= "Data Source")]
        public string DataSource { get; set; }
        public int MaxConnection { get; set; }
    }
	public class DbCfg : IDbCfg
    {
        public string Provider { get; set; }
        public string DataSource { get; set; }
		public int MaxConnection { get; set; }
	}

}


