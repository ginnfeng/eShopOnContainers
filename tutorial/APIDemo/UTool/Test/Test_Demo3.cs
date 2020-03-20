using System;
using UTDll;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
//using System.Xml.Query;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.Windows.Forms;
namespace UTool
{
	/// <summary>
	/// 
	/// </summary>
	public class Test_Demo3 : UTest
	{
		public Test_Demo3()
		{
			// 
			// TODO: Add constructor logic here
			//
		
		}
		[UMethod]
		public void plus(string a,string b)
		{
			StringBuilder ss=new StringBuilder();
			ss.Append(a);
			ss.Append(b);
			assert(ss.ToString() == (a+b));
		}
		[UMethod]
		public void UDataStore_save(string dir,string fileName)
		{
			UDataStore ds=new UDataStore(dir,fileName);
			ds.save("TestCase","testUDataStore",dir+","+fileName);
		}
		[UMethod]
		public void UDataStore_read(string dir,string fileName)
		{
			UDataStore ds=new UDataStore(dir,fileName);
			print("count",ds.count("TestCase"));
			for(int i=0;i<ds.count("TestCase");i++)
			{
				print(ds.read("TestCase",i));
			}
				
		}
		[UMethod]
		public StringReader  getXML(string fileName)
		{
			Assembly asm=Assembly.GetEntryAssembly();				
			Stream stream=asm.GetManifestResourceStream(asm.GetName().Name + "."+fileName);
			StreamReader reader=new StreamReader(stream);
			
			//string ss=reader.ReadToEnd();
			//messageBox(ss);
			return new StringReader(reader.ReadToEnd());
		}
		[UMethod]
		public void xslTrans()
		{
            /*
			//XmlTextReader tr = new XmlTextReader(getXML("report.xsl"));
            XsltCommand xslt = new XsltCommand();
            xslt.Compile(new XmlTextReader(getXML("report.xsl") )) ;
			XmlDocument doc = new XmlDocument();
			XmlTextReader xtr = new XmlTextReader(getXML("report.xml"));
			doc.Load(xtr);

			StringWriter writer = new StringWriter();
			
			//xslt.Transform(doc, null, writer);
			print(writer.ToString());
             */
		}
		[UMethod]
		public void testSerializable()
		{
					
			string file=USymbols.url_initData;
			
			XmlSerializer xs=new XmlSerializer(typeof(UVersion));
			
			UVersion ver=new UVersion(1000,"A","B");
			
			Stream fs=new FileStream("c:\\a.xml",FileMode.Create);
			xs.Serialize(fs,ver);
			fs.Close();
			
			
			XmlTextReader xtr = new XmlTextReader(file)	;
			
			
			UVersion ver2=(UVersion)xs.Deserialize(xtr);
			print(ver2.id);
			print(ver2.ver);
			print(ver2.url);
			print(ver2.regUrl);
			print(ver2.comment);
			
		}
		[UMethod]
		public void testRegexp(string exp, string str)
		{
			Regex regex = new Regex(exp);
			
			assert(regex.IsMatch(str));
		}
		[UMethod]
		public void Test_WebRequest(string postData)
		{
			WebRequest wr = WebRequest.Create("http://www28.brinkster.com/gfeng/UTool/Register.asp");
			wr.Method = "POST";
			wr.ContentType =
				"application/x-www-form-urlencoded";
			// Encode the data
			byte[] data =
				Encoding.ASCII.GetBytes(postData);
			wr.ContentLength = data.Length;
			// Build the body of the request
			Stream sReq = wr.GetRequestStream();
			sReq.Write(data, 0, data.Length);
			sReq.Close();
			wr.GetResponse();
		}
        [UMethod]
        public void t_Date()
        {// TODO: Add Testing logic here
            try
            {

                

                CultureInfo info = new CultureInfo(CultureInfo.CurrentCulture.Name, false);

                Application.CurrentCulture = info;
                

                DateTime d2 = new DateTime(1900, 1, 1);
                //DateTime d2 = DateTime.MinValue;


                print(d2.Year);
                print(d2.ToString());
                printf("{0}/{1}/{2}", d2.Year, d2.Month, d2.Day);
                print(d2.Date);
            }
            catch (Exception e)
            {
            }
        }        
	}
}
