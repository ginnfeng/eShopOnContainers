using System;
using UTDll;
using UTDll.support_lib;

using System.Security.Cryptography   ;
using System.Text ; 
using System.IO ; 
namespace UTool.Test
{
	/// <summary>
	/// Summary description for Test_Crypto.
	/// </summary>
	public class Test_Crypto:UTest
	{
		public Test_Crypto()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		[UMethod]
		public void t_crypto(string passKey,string txt,string key2)
		{
			Crypto cp=new Crypto(passKey);	
			string es=cp.encryptString(txt);
			
			Crypto cp2=new Crypto(key2);				
			string ds=cp2.decryptString(es);
			print(es);
			print(ds);
		}
		[UMethod]
		public void t_crypto2(string txt)
		{
			Crypto cp=new Crypto("passKey");	
			byte[] bts=Convert.FromBase64String(txt);
			//byte[] bts=cp.bytesFromString(txt);
			string s=cp.bytesToString(bts);
			print(s);
		}
		[UMethod]
		public void t_crypto3(string txt)
		{
			Crypto cp=new Crypto("passKey");	
			//byte[] bts=Convert.FromBase64String(txt);
			//byte[] bts=cp.bytesFromString(txt);
			
			byte[] bts=cp.bytesFromString(txt);
			MemoryStream plainStream=new MemoryStream(bts.Length);
			//plainStream.Write(byteArray, 0, iBytesIn);			
			
			plainStream.Write(bts, 0,(int) bts.Length);
			byte[] bts2=plainStream.GetBuffer();
			Array.Clear(bts2,(int)plainStream.Length,(int) (bts2.Length-plainStream.Length) );
			string s=cp.bytesToString(bts);
			print(s);
			s=cp.bytesToString(bts2);
			print(s);
		}

	}
}
