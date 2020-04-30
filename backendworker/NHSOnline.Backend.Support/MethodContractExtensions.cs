using System;

namespace NHSOnline.Backend.Support
{
    public static class MethodContractExtensions
    {
        public static void AssertArgumentNotNull(this object self, string name)
        {
            if (self is null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}