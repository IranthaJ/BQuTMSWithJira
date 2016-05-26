using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BQuTMSWithJira
{
    //get path for Application Data folder of any machin 
    class Util
    {
        public static string GetUserDataPath()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir = System.IO.Path.Combine(dir, "BQuTMSWithJira");
            if (!Directory.Exists(dir))
                System.Windows.Forms.MessageBox.Show("Not found BQuTMSWithJira folder...!", "BQuTMSWithJira", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore, System.Windows.Forms.MessageBoxIcon.Error);
            return dir;
        }

        public static DateTime getDateTime()
        {
            return DateTime.Now;
        }

        public static string DecryptDES(string cryptedString)//decript server password using DES algorithm
        {
            //console.writeline("cryptedString " + cryptedString);
            if (!String.IsNullOrEmpty(cryptedString))
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes("teeeeeet");//this is the key which used for encription and decription
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
            else
            {
                return "";
            }
        }

        public static string EncryptDES(string originalString)
        {
            if (!String.IsNullOrEmpty(originalString))
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes("teeeeeet");//this is the key which used for encription and decription
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

                StreamWriter writer = new StreamWriter(cryptoStream);
                writer.Write(originalString);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
            else
            {
                return "";
            }
        }

    }
}
