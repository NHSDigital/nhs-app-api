using Org.BouncyCastle.OpenSsl;

namespace NHSOnline.Backend.Support.Certificate
{
    internal class PasswordFinder : IPasswordFinder
    {
        private readonly string _password;

        public PasswordFinder(string password)
        {
            _password = password;
        }

        public char[] GetPassword()
        {
            return _password.ToCharArray();
        }
    }
}