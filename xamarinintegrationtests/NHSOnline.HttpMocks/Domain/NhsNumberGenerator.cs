using System.Threading;

namespace NHSOnline.HttpMocks.Domain
{
    internal sealed class NhsNumberGenerator
    {
        private int _nextNumber = 1001001000;

        internal NhsNumber Next()
        {
            var number = Interlocked.Increment(ref _nextNumber);
            return NhsNumber.FromInt(number);
        }
    }
}