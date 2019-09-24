using NHSOnline.Backend.UsersApi.Areas.Devices;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class DeviceRegistrationResult
    {
        public abstract T Accept<T>(IDeviceRegistrationResultVisitor<T> visitor);

        public class Created : DeviceRegistrationResult
        {
            public UserDevice UserDevice { get; }

            public Created(UserDevice userDevice)
            {
                UserDevice = userDevice;
            }

            public override T Accept<T>(IDeviceRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : DeviceRegistrationResult
        {
            public override T Accept<T>(IDeviceRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : DeviceRegistrationResult
        {
            public override T Accept<T>(IDeviceRegistrationResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}