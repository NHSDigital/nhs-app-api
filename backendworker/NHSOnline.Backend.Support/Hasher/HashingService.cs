using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Hasher
{
    public interface IHashingService
    {
        // If this flag is true, it should be considered that the
        // hashing service in this API instance is in an error state
        // that cannot be recovered from.
        bool IsDead { get; }
        string Hash(string valueToEncrypt);
    }

    public class HashingService : IHashingService
    {
        private readonly ISha512ProviderFactory _sha512ProviderFactory;
        private readonly ILogger _logger;

        private readonly object _sha512ProviderLock;
        private ISha512Provider _sha512Provider;

        public bool IsDead { get; private set; }

        public HashingService(ISha512ProviderFactory sha512ProviderFactory, ILogger<HashingService> logger)
        {
            _sha512ProviderFactory = sha512ProviderFactory;
            _logger = logger;

            _sha512ProviderLock = new object();
            _sha512Provider = _sha512ProviderFactory.Build();

            IsDead = false;
        }

        public string Hash(string valueToEncrypt) => DoHash(valueToEncrypt, false);

        private string DoHash(string valueToEncrypt, bool isRetry)
        {
            var bytesToEncrypt = Encoding.UTF8.GetBytes(valueToEncrypt);

            try
            {
                byte[] hash;

                lock(_sha512ProviderLock)
                {
                    hash = _sha512Provider.ComputeHash(bytesToEncrypt);
                }

                return Convert.ToBase64String(hash, 0, hash.Length);
            }
            catch(CryptographicException ex)
            {
                _logger.LogError(ex, "Sha512Provider threw a CryptographicException");

                if (!isRetry)
                {
                    // This hash function is called by user registration journeys - if this fails
                    // EMIS users could get stuck in a `limbo` state, so we should retry at least
                    // once.
                    return RebuildProviderAndRetryHash(valueToEncrypt);
                }

                _logger.LogError("Sha512Provider hash retry failed, marking service as unhealthy");

                IsDead = true;
                throw;
            }
        }

        private string RebuildProviderAndRetryHash(string valueToEncrypt)
        {
            lock (_sha512ProviderLock)
            {
                try
                {
                    _sha512Provider.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error disposing of Sha512Provider");
                }

                _sha512Provider = _sha512ProviderFactory.Build();
            }

            _logger.LogWarning("Retrying Sha512Provider hash operation with new instance");

            return DoHash(valueToEncrypt, true);
        }
    }
}
