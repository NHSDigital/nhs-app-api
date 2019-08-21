using System;
using System.Security.Cryptography;

namespace NHSOnline.Backend.Support
{
    public interface IRandomStringGenerator
    {
        string GenerateString(int size, string characters);
    }
    
    public class RandomStringGenerator : IRandomStringGenerator, IDisposable
    {
        private readonly RandomNumberGenerator _rng;
        private bool _disposed;

        public RandomStringGenerator(RandomNumberGenerator rng)
        {
            _rng = rng;
        }
        
        public string GenerateString(int size, string characters)
        {   
                var data = new byte[size];

                // buffer used if we encounter an unusable random byte. We will
                // regenerate it in this buffer
                byte[] smallBuffer = null;

                // Maximum random number that can be used without introducing a
                // bias
                var maxRandom = byte.MaxValue - (byte.MaxValue + 1) % characters.Length;
                
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                
                _rng.GetBytes(data);

                var result = new char[size];

                for (var i = 0; i < size; i++)
                {
                    var v = data[i];

                    while (v > maxRandom)
                    {
                        if (smallBuffer == null)
                        {
                            smallBuffer = new byte[1];
                        }

                        _rng.GetBytes(smallBuffer);
                        v = smallBuffer[0];
                    }

                    result[i] = characters[v % characters.Length];
                }
                return new string(result);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RandomStringGenerator()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _rng.Dispose();
            }

            _disposed = true;
        }
    }
}