using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.GpSystems
{
    [Serializable]
    public class InvalidPatientIdException : Exception
    {
        public InvalidPatientIdException(Guid patientIdHeader)
            : base($"Patient id ({patientIdHeader}) did not match logged in user's id or id of any of their proxies.")
        {
        }

        public InvalidPatientIdException()
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
