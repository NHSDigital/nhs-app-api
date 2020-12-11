using NHSOnline.Backend.Support.Cipher;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    /// <summary>
    /// Serializes and encrypts a Im1 Token for storing the in the cache.
    /// </summary>
    internal sealed class Im1TokenEncryptionService
    {
        private readonly Im1TokenSerialiserService _serialiserService;
        private readonly ICipherService _cipherService;

        public Im1TokenEncryptionService(
            Im1TokenSerialiserService serialiserService,
            ICipherService cipherService)
        {
            _serialiserService = serialiserService;
            _cipherService = cipherService;
        }

        internal string Encode<T>(T token)
        {
            var sessionObject = _serialiserService.Serialise(token);
            return _cipherService.Encrypt(sessionObject);
        }

        internal T Decode<T>(string encodedIm1Token)
        {
            var tokenJson = _cipherService.Decrypt(encodedIm1Token);
            return _serialiserService.Deserialise<T>(tokenJson);
        }
    }
}