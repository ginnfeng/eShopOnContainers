using System;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;

namespace Common.Support
{
    public class FtpMgr
    {
        private string m_strAddress;
        private string m_uid;
        private string m_pwd;
        private string m_ip;
        public FtpMgr(string ip, string userId, string password)
        {
            m_ip = ip;
            m_uid = userId;
            m_pwd = password;
        }
        public void Upload(string msg, string fileName)
        {
            m_strAddress = String.Format(CultureInfo.CurrentCulture,@"ftp://{0}/{1}", m_ip, fileName);
            Upload(msg);
        }
        private void Upload(string msg)
        {
            //string strAddress = @"ftp://10.144.4.183/out.xml";
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.contoso.com/test.htm");
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_strAddress);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            //request.Credentials = new NetworkCredential("oh", "passwd4oh");
            request.Credentials = new NetworkCredential(m_uid, m_pwd);
            
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(msg));
            StreamReader sourceStream = new StreamReader(ms);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

    }
}
