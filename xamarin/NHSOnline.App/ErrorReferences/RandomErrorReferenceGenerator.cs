using System.Security.Cryptography;

namespace NHSOnline.App
{
    public static class RandomErrorReferenceGenerator
    {
        public static string GenerateString(int size, string characters)
        {
            var data = new byte[size];

            var maxRandom = byte.MaxValue - (byte.MaxValue + 1) % characters.Length;

            using var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(data);

            return string.Create(size, randomNumberGenerator, (result, rng) =>
            {

                byte[]? smallBuffer = null;

                for (var i = 0; i < size; i++)
                {
                    var v = data[i];

                    while (v > maxRandom)
                    {
                        if (smallBuffer == null)
                        {
                            smallBuffer = new byte[1];
                        }

                        rng.GetBytes(smallBuffer);
                        v = smallBuffer[0];
                    }

                    result[i] = characters[v % characters.Length];
                }
            });
        }
    }
}