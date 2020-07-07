using System;
using System.Runtime.Serialization;
using NHSOnline.Backend.GpSystems.Session;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Session
{
    [Serializable]
    public class EmisSessionResponseErrorException : Exception
    {
        public GpSessionCreateResult ErrorResult{ get; }

        public EmisSessionResponseErrorException()
        {
        }

        public EmisSessionResponseErrorException(GpSessionCreateResult errorResult)
        {
            ErrorResult = errorResult;
        }

        public EmisSessionResponseErrorException(string message) : base(message)
        {
            ErrorResult = new GpSessionCreateResult.ErrorExceptionResult(message);
        }

        public EmisSessionResponseErrorException(string message, Exception inner) : base(message, inner)
        {
            ErrorResult = new GpSessionCreateResult.ErrorExceptionResult(message);
        }

        protected EmisSessionResponseErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            ErrorResult = new GpSessionCreateResult.ErrorExceptionResult("Unknown error");
        }
    }
}