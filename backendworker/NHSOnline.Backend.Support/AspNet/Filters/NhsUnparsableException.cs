using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    public class NhsUnparsableException : FormatException
    {
        public List<NhsUnparsableExceptionError> ErrorMessages { get; }

        public NhsUnparsableException()
        {
        }

        public NhsUnparsableException(string message) : base(message)
        {
        }

        public NhsUnparsableException(string message, Exception innerException) : base(message, innerException)
        {
        }


        public NhsUnparsableException(string message, IEnumerable<NhsUnparsableExceptionError> errorMessages) : base(message)
        {
            ErrorMessages = errorMessages.ToList();
        }
    }
}