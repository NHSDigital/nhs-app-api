using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Support
{
    [Serializable]
    public class InvalidPatientIdException : Exception
    {
        public InvalidPatientIdException()
            : base($"Patient id did not match logged in user's id or id of any of their proxies.")
        {
        }
        public InvalidPatientIdException(string message) : base(message)
        {
        }

        public InvalidPatientIdException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidPatientIdException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
