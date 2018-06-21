using System;
using System.Security.Cryptography;
using System.Text;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class AuditCryptographer
    {
        public static string Hash(string valueToEncrypt)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(valueToEncrypt);
            var provider = new SHA512CryptoServiceProvider();
            var hash = provider.ComputeHash(bytesToEncrypt);

            return $"[{Convert.ToBase64String(hash, 0, hash.Length)}]";
        }
    }
}
