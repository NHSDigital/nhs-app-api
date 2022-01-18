using System;

namespace NHSOnline.App.Logging
{
    public class GetConfigurationException : Exception
    {

        private GetConfigurationException()
        {
        }

        public GetConfigurationException(string message) : base(message)
        {
        }

        private GetConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}