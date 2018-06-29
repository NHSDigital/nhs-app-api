using System.IO;
using System.Security.Cryptography;
using System.Text;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Support.Cipher
{
    public interface ICipherService
    {
        string Encrypt(string input);
        string Decrypt(string cipherText);
    }

    public class CipherService : ICipherService
    {
        private readonly byte[] _aesKey;

        public CipherService(CipherConfiguration configuration)
        {
            var keyFileContents = File.ReadAllText(configuration.CipherKeyFilePath);
            _aesKey = Encoding.UTF8.GetBytes(keyFileContents);
        }

        public string Decrypt(string encryptedValueString)
        {
            var encryptedValue = EncryptedValue.Parse(encryptedValueString);

            using (var aes = Aes.Create())
            using (var decryptor = aes.CreateDecryptor(_aesKey, encryptedValue.InitialisationVector))
            {
                var textDataBytes = decryptor.TransformFinalBlock(encryptedValue.EncryptedData, 0, encryptedValue.EncryptedData.Length);
                return Encoding.UTF8.GetString(textDataBytes);
            }
        }

        public string Encrypt(string input)
        {
            var textDataBytes = Encoding.UTF8.GetBytes(input);
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                var initialisationVector = aes.IV;

                using (var encryptor = aes.CreateEncryptor(_aesKey, initialisationVector))
                {
                    var encryptedBytes = encryptor.TransformFinalBlock(textDataBytes, 0, textDataBytes.Length);
                    return new EncryptedValue(initialisationVector, encryptedBytes).ToString();
                }
            }
        }
    }
}
