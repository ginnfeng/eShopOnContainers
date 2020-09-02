////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 3/31/2009 4:53:57 PM 
// Description: Crypto.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
//using Support.Helper;

namespace Common.Support.Net.Security
{
    public class Crypto : Crypto<AesManaged>//or RijndaelManaged 
    {
        public Crypto()
        {
        }
        public Crypto(string cryptoPassPhrase)
            :base(cryptoPassPhrase)
        {
        }
    }
    public class Crypto<TEncrypManagedImp>
        where TEncrypManagedImp : SymmetricAlgorithm,new()
    {
        public Crypto()
        {
            PassPhrase = defaultPassPhrase;
        }
        public Crypto(string cryptoPassPhrase)
        {
            PassPhrase = cryptoPassPhrase;
        }
        static public Crypto<TEncrypManagedImp> TodayCrypto
        {
            get
            {
                var passPhrase = DateTime.Now.Date.ToFileTimeUtc().ToString() + defaultPassPhrase;
                if(todayCrypto==null || !todayCrypto.PassPhrase.Equals(passPhrase))
                    todayCrypto=new Crypto<TEncrypManagedImp>(passPhrase);
                return todayCrypto;
            }
        }
        static Crypto<TEncrypManagedImp> todayCrypto;

        //generate and store encryption key & iv from passphrase
        public string PassPhrase
        {
            get
            {
                return passPhrase;
            }
            set
            {
                const int iMinLength = -1;//   '-1 disables min length
                passPhrase = value.Trim();

                //enforce a rule on minimum length if desired here
                if ((value.Length > iMinLength) || (iMinLength == -1))
                {
                    SHA256Managed sha2 = new SHA256Managed();	//256 bits = 32 byte key
                    cryptoKey = sha2.ComputeHash(BytesFromString(passPhrase, StringEncoding));
                    //convert to Base64 for Initialization Vector, take last 16 chars
                    string sKey = Convert.ToBase64String(cryptoKey);
                    //initVector = Encoding.ASCII.GetBytes(sKey.Remove(0, sKey.Length - 16));
                    initVector = Encoding.UTF8.GetBytes(sKey.Remove(0, sKey.Length - 16));
                }
                else
                {
                    throw new Exception(String.Format("PassPhrase length must be at least {0} characters.", (iMinLength + 1).ToString()));
                }
            }
        }

        //encrypt a file - replaces input file
        public bool EncryptFile(string targetFile)
        {
            return DoEncryptFile(targetFile, targetFile);
        }

        //encrypt a file - separate output file												
        public bool EncryptFile(string plainFile, string encryptedFile)
        {
            return DoEncryptFile(plainFile, encryptedFile);
        }

        public byte[] Encrypt(byte[] byteArray)
        {
            using (MemoryStream plainStream = new MemoryStream(byteArray))
            using (MemoryStream encryptStream = EncryptStream(plainStream))
            {
                return encryptStream.ToArray();
            }
        }
        
        public void Encrypt<T>(ref T it)
        {
            DoCrypt<T>(ref it, value => this.Encrypt(value));
        }

        //encrypt a stream
        public TStream EncryptStream<TStream>(TStream plainStream)
            where TStream:Stream,new()
        {          
            //open stream for encrypted data
            TStream encStream = new TStream();
            //create Crypto Service Provider, set key, transform and crypto stream
            
            //using (RijndaelManaged oCSP = new RijndaelManaged())
            using (TEncrypManagedImp oCSP = new TEncrypManagedImp())
            {
                oCSP.Key = cryptoKey;
                oCSP.IV = initVector;
                ICryptoTransform cryptoTransform = oCSP.CreateEncryptor();
                using (CryptoStream cryptoStream = new CryptoStream(encStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    //get input stream into byte array
                    plainStream.Position = 0;
                    byte[] byteArray = ReadAllBytes(plainStream);
                    //write input bytes to crypto stream and close up
                    cryptoStream.Write(byteArray, 0, (int)plainStream.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                    return encStream;
                }
            }
        }

        public Byte[] Decrypt(byte[] encryptedByteArray)
        {
            using (MemoryStream encryptStream = new MemoryStream(encryptedByteArray))
            using (MemoryStream decryptStream = DecryptStream(encryptStream))
            {
                return decryptStream.ToArray();
            }
        }

        public void Decrypt<T>(ref T it)
        {
            DoCrypt<T>(ref it, value => this.Decrypt(value));
        }

        //decrypt a stream
        public TStream DecryptStream<TStream>(TStream encryptedStream)
            where TStream:Stream,new()
        {           
            //create Crypto Service Provider, set key, transform and crypto stream
            
            using (TEncrypManagedImp oCSP = new TEncrypManagedImp())
            {
                oCSP.Key = cryptoKey;
                oCSP.IV = initVector;
                ICryptoTransform cryptoTransform = oCSP.CreateDecryptor();
                using (CryptoStream cryptoStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    //get bytes from encrypted stream
                    byte[] byteArray = new byte[encryptedStream.Length];
                    int iBytesIn = cryptoStream.Read(byteArray, 0, (int)encryptedStream.Length);                    
                    //create and write the decrypted output stream
                    TStream plainStream = new TStream();
                    Array.Resize<byte>(ref byteArray, iBytesIn);
                    plainStream.Write(byteArray, 0, iBytesIn);
                    
                    //byte[] bts=this.bytesFromString("abc\0");
                    //plainStream.Write(bts, 0,(int) bts.Length);
                    plainStream.Position = 0;
                    return plainStream;
                }
            }
        }
        //encrypt a string - wrapper without Base64 flag (True by default)
        public string Encrypt(string plainText)
        {           
            return DoEncryptString(plainText);
        }

        //decrypt a file - replaces input file
        public bool DecryptFile(string targetFile)
        {            
            return DoDecryptFile(targetFile, targetFile);
        }
        //decrypt a file - separate output file
        public bool Decrypt(string encryptedFile, string plainFile)
        {            
            return DoDecryptFile(encryptedFile, plainFile);
        }

        //decrypt a string - wrapper without Base64 flag (True by default)
        public string Decrypt(string encryptedString)
        {            
            return DoDecryptString(encryptedString);
        }
       
        //calculates the hash of InputValue, returns a string
        //- SHA1 hash is always 20 bytes (160 bits)        
        static public string GetHashString(string inputValue,Encoding encoding)
        {
            byte[] inputBytes = BytesFromString(inputValue, encoding);
            byte[] hashValue = new SHA1Managed().ComputeHash(inputBytes);
            return BytesToHexString(hashValue);//return 40-byte hex string
        }

        //returns True if hash of Passphrase matches HashValue
        public bool ValidPassword(string passphrase, string hashValue)
        {
            if (passphrase == null || hashValue == null)
                return false;
            return (GetHashString(passphrase) == hashValue);
        }

        public string GetHashString(string word)
        {
            return GetHashString(word, StringEncoding);
        }
        /// <summary>
        /// 加密過的字串經http(含http://或WebService)傳送其'+'字元都會被置換為' '
        /// </summary>
        /// <param name="encryptedStr"></param>
        /// <returns></returns>
        static public string CompensateHttp(string encryptedStr)
        {
            return encryptedStr.Replace(' ', '+');
        }
        //--------------------------------------private--------------------------------------------
        //returns a Unicode byte array from a string
        static public byte[] BytesFromString(string str,Encoding encoding)
        {
            //return (new UnicodeEncoding()).GetBytes(str);
            //return (new UnicodeEncoding()).GetBytes(str);
            return encoding.GetBytes(str);
        }

        //internal file encryption
        private bool DoEncryptFile(string plainFile, string encryptedFile)
        {

            //set flag for replacement if filenames are the same, open files
            bool bReplaceFile = (encryptedFile.ToLower().Trim() == plainFile.ToLower().Trim());
            string sEncryptedFile = (bReplaceFile) ? Path.GetTempFileName() : encryptedFile;
            using (FileStream fsIn = File.OpenRead(plainFile))
            {  
                using (FileStream fsOut = File.OpenWrite(sEncryptedFile))
                {
                    byte[] bytesPlain = ReadAllBytes(fsIn);
                    byte[] bytesEncrypt = Encrypt(bytesPlain);
                    fsOut.Write(bytesEncrypt, 0, bytesEncrypt.Length);                    
                }
            }
            //replace input file if flag set
            if (bReplaceFile)
                return ReplaceFile(sEncryptedFile, plainFile);

            return true;
            
        }

        //internal file decryption method
        private bool DoDecryptFile(string encryptedFile, string plainFile)
        {
            //set flag for replacement if filenames are the same, open input file
            bool bReplaceFile = (encryptedFile.ToLower().Trim() == plainFile.ToLower().Trim());
            //create and write the decrypted output file
            string sPlainFile = (bReplaceFile) ? Path.GetTempFileName() : plainFile;

            using (FileStream fsIn = File.OpenRead(encryptedFile))
            using(FileStream fsOut = File.OpenWrite(sPlainFile))
            {
                byte[] bytesEncrypt = ReadAllBytes(fsIn);
                byte[] bytesPlain = Decrypt(bytesEncrypt);
                fsOut.Write(bytesPlain, 0, bytesPlain.Length);               
            }
            //replace input file if flag set
            if (bReplaceFile)
                return ReplaceFile(sPlainFile, encryptedFile);
            return true;
        }

        //internal string decryption
        private string DoDecryptString(string encryptedString)
        {
            //put string in byte array depending on Base64 flag
            byte[] byteArray = Convert.FromBase64String(encryptedString);

            //create the streams, decrypt and return a string
            using(MemoryStream msEnc = new MemoryStream(byteArray))
            using (MemoryStream msPlain = DecryptStream(msEnc))
            {
                return BytesToString(msPlain.ToArray());
            }
        }

        //internal string encryption
        private string DoEncryptString(string plainText)
        {
            //put string in byte array
            byte[] byteArray = BytesFromString(plainText, StringEncoding);
            //create streams and encrypt
            using(MemoryStream msPlain = new MemoryStream(byteArray))
            using (MemoryStream msEnc = EncryptStream(msPlain))
            {
                //return string depending on Base64 flag
                return Convert.ToBase64String(msEnc.ToArray());
            }
        }

        //replace a file with the contents of another
        private bool ReplaceFile(string tempFile, string targetFile)
        {
            File.Copy(tempFile, targetFile, true);
            //delete the temp file
            File.Delete(tempFile);
            return true;
        }
        //returns a hex string from a byte array
        static private string BytesToHexString(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder(40);
            foreach (byte bValue in byteArray)
            {
                sb.AppendFormat(bValue.ToString("x2").ToUpper());
            }
            return sb.ToString();
        }

        //returns a Unicode string from a byte array 
        public string BytesToString(byte[] byteArray)
        {
            return StringEncoding.GetString(byteArray, 0, byteArray.Count());
        }

        public Encoding StringEncoding 
        {
            get { return encoding; }
            set { encoding = value; } 
        }

        public void DoCrypt<T>(ref T it, Func<string, string> crypt)
        {
            Type type = typeof(T);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.CanRead || !propertyInfo.CanWrite || propertyInfo.PropertyType != typeof(string))
                    continue;
                object value = propertyInfo.GetValue(it, null);
                if (value != null)
                {
                    propertyInfo.SetValue(it, crypt(value.ToString()), null);
                }
            }
        }
        public static byte[] ReadAllBytes(Stream stream)
        {
            // Use this method is used to read all bytes from a stream.
            using (MemoryStream tempStream = new MemoryStream())
            {
                //某些Stream例如GZipStream不提供Position property
                //stream.Position = 0;
                byte[] bytes = new byte[4096];
                int n;
                while ((n = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    tempStream.Write(bytes, 0, n);
                }
                return tempStream.ToArray();
            }
        }
        //.............................................
        private Encoding encoding = Encoding.UTF8;//Encoding.Unicode;
        private string passPhrase;
        private byte[] cryptoKey; //crypto key
        private byte[] initVector; //Initialization Vector 

        private const string keyNotSetException = "Crypto passphrase is not set.";
        private const string defaultPassPhrase = "{F71D5F85-7877-426c-90F5-61787C9C0B75}";
    }

}
