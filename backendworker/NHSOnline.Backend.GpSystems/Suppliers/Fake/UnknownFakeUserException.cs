using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake
{
    [Serializable]
    public class UnknownFakeUserException : Exception
    {
        public UnknownFakeUserException()
        {
        }

        public UnknownFakeUserException(string message) : base(message)
        {
        }

        public UnknownFakeUserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownFakeUserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}