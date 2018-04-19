using Microsoft.AspNetCore.DataProtection;

namespace NHSOnline.Backend.Worker.DataProtection
{
    public class CipherService : ICipherService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string ProtectorName = "SessionProtector";

        public CipherService(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(ProtectorName);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(ProtectorName);
            return protector.Unprotect(cipherText);
        }
    }
}
