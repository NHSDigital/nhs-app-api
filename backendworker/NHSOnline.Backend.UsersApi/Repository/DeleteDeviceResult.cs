using NHSOnline.Backend.UsersApi.Areas.Devices;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class DeleteDeviceResult
    {
        public class Success : DeleteDeviceResult
        {
            public string DeviceId { get; }

            public Success(string deviceId)
            {
                DeviceId = deviceId;
            }
            
            public override T Accept<T>(IDeleteDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : DeleteDeviceResult
        {
            public override T Accept<T>(IDeleteDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : DeleteDeviceResult
        {
            public override T Accept<T>(IDeleteDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public abstract T Accept<T>(IDeleteDeviceResultVisitor<T> visitor);
    }
}