using System.Globalization;
using System.Security.Cryptography;
using System.Threading;

namespace NHSOnline.HttpMocks.Domain
{
    internal sealed class NhsLoginIdGenerator
    {
        private string _baseGuid = "c9101a74-85e5-4fbf-86a1-d19c12";
        private int _nextNumber = RandomNumberGenerator.GetInt32(100000, 1000000);

        internal string Next()
        {
            var number = Interlocked.Increment(ref _nextNumber);
            var formattedStringValue = number.ToString("000000", CultureInfo.InvariantCulture);
            return $"{_baseGuid}{formattedStringValue}";
        }
    }
}