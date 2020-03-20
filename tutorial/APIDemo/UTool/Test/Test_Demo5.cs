using System;
using System.Threading;
using UTDll;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UTool.Test
{
	/// <summary>
	/// Summary description for Test_Demo.
	/// </summary>
	
	public class Test_Demo5:UTest
	{
		public Test_Demo5()
		{
			//
			// TODO: Add constructor logic here
			//
		}
        static Regex regex = new Regex("\"[0-9,]{1,}\"");
  
        [UMethod]
		public void T1()
		{
            
            List<string> list = new List<string>();
            var cList = new List<double>();
            using (var reader = new StreamReader(File.OpenRead(@"C:\ProgNet\UTool\Test\4938_a.csv")))
            {
                while (!reader.EndOfStream)
                {

                    var line = reader.ReadLine();
                    var matchs = regex.Match(line);
                    if (matchs.Success)
                    {
                        var s = regex.Replace(line, delegate (Match match)
                              {
                                  string v = match.ToString();
                                  return v.Replace(",", "");
                              });
                        s = s.Replace("\"", "").Replace("+", "");
                        list.Add(s);
                        var values = s.Split(',');
                        cList.Add(Convert.ToDouble(values[6]));
                    }

                }
            }
            int maxIdx = list.Count - 1;
            int ln = 20;
            for(int i=0;i< maxIdx-ln;i++)
            {
                var c = cList[i];
                var vv = GetMinMax(cList,c,i+1,i+1+ln);
                var s = list[i];
                list[i] = s+"," + vv.ToString();
            }
            using (var writer = new StreamWriter(File.Create(@"C:\ProgNet\UTool\Test\4938_2.csv")))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }
        private int GetMinMax(List<double> list, double it,int from,int to)
        {
            double max = 0, min = 999;
            for(int i= from;i<= to;i++)
            {
                var v = list[i];
                if (v>it)
                {
                    if (v > max) max = v;
                }else
                {
                    if (v < min) min = v;
                }
            }
            return (max - it) > (it - min) ? (int)(((max - it)/ it)*100) : (int)(((it-min) / it) * (-100));
        }
		[UMethod]
		public void T2()
		{	
		}
		[UMethod]
		public void T3(string s)
		{		
			
           
		}
        [UMethod]
        public void T4(string s)
        {
            
        }
	}
}
