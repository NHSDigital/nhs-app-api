using System;
using System.Security.Cryptography;
using System.Text;

namespace NHSOnline.Backend.Worker.Support.Hasher
{
    public interface IHashingService
    {
        string Hash(string valueToEncrypt);
    }
    
    public class HashingService : IHashingService
    {
        

        public string Hash(string valueToEncrypt)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(valueToEncrypt);
            var provider = new SHA512CryptoServiceProvider();
            var hash = provider.ComputeHash(bytesToEncrypt);

            return Convert.ToBase64String(hash, 0, hash.Length);            
        }
        
    }
}