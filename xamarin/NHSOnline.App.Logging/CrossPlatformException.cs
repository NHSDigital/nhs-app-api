using System;

namespace NHSOnline.App.Logging
{
    public class CrossPlatformException: Exception
    {

        public CrossPlatformErrorType ErrorType { get; private set; }

        private CrossPlatformException()
        {
        }

        private CrossPlatformException(string message) : base(message)
        {
        }

        private CrossPlatformException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CrossPlatformException(string message, CrossPlatformErrorType errorType) : base(message)
        {
            ErrorType = errorType;
        }
    }

    public enum CrossPlatformErrorType
    {
        UnrecoverableKey,
    }
}