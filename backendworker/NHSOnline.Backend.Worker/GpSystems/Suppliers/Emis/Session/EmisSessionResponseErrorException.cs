using System;
using System.Runtime.Serialization;
using NHSOnline.Backend.Worker.GpSystems.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session
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
        }

        public EmisSessionResponseErrorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EmisSessionResponseErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}