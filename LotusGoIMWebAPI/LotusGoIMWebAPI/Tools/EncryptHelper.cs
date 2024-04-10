using System.Security.Cryptography;
using System.Text;

namespace LotusGoIMWebAPI.Common
{
    public class EncryptHelper
    {
        public static string EncryptPassword(string password)
        {
            string salt = "LotusGoIM";
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(password+salt));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "").ToLower();
        }
    }
}
