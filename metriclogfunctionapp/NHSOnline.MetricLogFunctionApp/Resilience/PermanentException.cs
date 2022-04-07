using System;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    internal sealed class PermanentException : Exception
    {
        public PermanentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PermanentException(string message) : base(message)
        {
        }
    }
}