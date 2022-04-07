using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Users.Notifications
{
    [Serializable]
    public class InstallationNotFoundException : Exception
    {
        public InstallationNotFoundException()
        {
        }

        public InstallationNotFoundException(string message) : base(message)
        {
        }

        public InstallationNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InstallationNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}