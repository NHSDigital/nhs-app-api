using System;

namespace NHSOnline.Backend.GpSystems
{
    public class NoGpSessionAvailableException : InvalidOperationException
    {
        public NoGpSessionAvailableException(
            string message = "Current P9 session does not have a valid GP user session") : base(message)
        {
        }

        public NoGpSessionAvailableException()
        {
        }

        public NoGpSessionAvailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}