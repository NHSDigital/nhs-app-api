namespace NHSOnline.Backend.Worker.DataProtection
{
    public interface ICipherService
    {
        string Encrypt(string input);
        string Decrypt(string cipherText);
    }
}
