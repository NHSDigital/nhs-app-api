using System;

namespace NHSOnline.Backend.Support
{
    public static class DocumentHelpers
    {
        public static double CalculateSizeFromDataLength(string data)
        {
            return Math.Ceiling((double) data.Length / 4) * 3;
        }
    }
}