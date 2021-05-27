using System;
using System.Security.Cryptography;

namespace NHSOnline.Backend.Support.Hasher
{
    public interface ISha512Provider : IDisposable
    {
        byte[] ComputeHash(byte[] subject);
    }

    public class Sha512Provider : ISha512Provider
    {
        private SHA512CryptoServiceProvider _provider;

        public Sha512Provider()
        {
            _provider = new SHA512CryptoServiceProvider();
        }

        public byte[] ComputeHash(byte[] subject) => _provider.ComputeHash(subject);

        public void Dispose()
        {
            _provider?.Dispose();
            _provider = null;

            GC.SuppressFinalize(this);
        }
    }
}