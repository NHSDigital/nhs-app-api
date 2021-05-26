using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Hasher
{
    public interface IHashingService
    {
        bool IsHealthy { get; set; }
        string Hash(string valueToEncrypt);
    }
    
    public class HashingService : IHashingService
    {
        private readonly SHA512CryptoServiceProvider _provider;
        private readonly ILogger _logger;

        public bool IsHealthy { get; set; }

        public HashingService(SHA512CryptoServiceProvider provider,
            ILogger<HashingService> logger) {
            _provider = provider;
            _logger = logger;
            IsHealthy = true;
        }

        public string Hash(string valueToEncrypt)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(valueToEncrypt);

            try
            {
                byte[] hash;

                lock(_provider)
                {
                    hash = _provider.ComputeHash(bytesToEncrypt);
                }

                return Convert.ToBase64String(hash, 0, hash.Length);
            }
            catch(CryptographicException)
            {
                _logger.LogError("SHA512CryptoServiceProvider threw a CryptographicException");

                IsHealthy = false;
                throw;
            }         
        }
        
    }
}