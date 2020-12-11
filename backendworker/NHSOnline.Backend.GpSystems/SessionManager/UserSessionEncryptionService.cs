using NHSOnline.Backend.Support.Cipher;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    /// <summary>
    /// Serializes and encrypts a user session for storing the in the session cache.
    /// </summary>
    internal sealed class UserSessionEncryptionService
    {
        private readonly UserSessionSerialiserService _serialiserService;
        private readonly ICipherService _cipherService;

        public UserSessionEncryptionService(
            UserSessionSerialiserService serialiserService,
            ICipherService cipherService)
        {
            _serialiserService = serialiserService;
            _cipherService = cipherService;
        }

        internal string Encode(UserSession userSession)
        {
            var sessionObject = _serialiserService.Serialise(userSession);
            return _cipherService.Encrypt(sessionObject);
        }

        internal UserSession Decode(string encodedUserSession)
        {
            var sessionJson = _cipherService.Decrypt(encodedUserSession);
            return _serialiserService.Deserialise(sessionJson);
        }
    }
}