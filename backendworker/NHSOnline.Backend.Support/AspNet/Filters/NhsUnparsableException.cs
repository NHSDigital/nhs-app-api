using System;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    public class NhsUnparsableException : FormatException
    {
        public SourceApi SourceApi { get; set; }

        public NhsUnparsableException()
        {
        }

        public NhsUnparsableException(string message) : base(message)
        {
        }

        public NhsUnparsableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}