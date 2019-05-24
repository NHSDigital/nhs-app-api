using System;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Exceptions
{
    public class FailedLoadJourneysException : Exception
    {
        public FailedLoadJourneysException()
        {
        }
        
        public FailedLoadJourneysException(string message) : base(message)
        {
        }

        public FailedLoadJourneysException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}