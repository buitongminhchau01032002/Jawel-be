using System.Security.Cryptography;
using System.Text;

namespace Jawel_be.Utils
{
    public static class HashPassword
    {
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString();

            }
            return byte2String;
        }
    }
}
