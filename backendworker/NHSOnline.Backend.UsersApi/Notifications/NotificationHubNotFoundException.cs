using System;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    [Serializable]
    public class NotificationHubNotFoundException :Exception
    {
        public NotificationHubNotFoundException(string message) : base(message)
        {
            
        }

        public NotificationHubNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
            
        }

        public NotificationHubNotFoundException()
        {
            
        }

        protected NotificationHubNotFoundException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
        :base(serializationInfo,streamingContext){
            
        }
    }
}