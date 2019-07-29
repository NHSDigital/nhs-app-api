namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class DeviceRepositoryResult
    {
        public abstract T Accept<T>(IDeviceRepositoryResultVisitor<T> visitor);

        public class Created : DeviceRepositoryResult
        {
            public UserDevice UserDevice { get; }

            public Created(UserDevice userDevice)
            {
                UserDevice = userDevice;
            }

            public override T Accept<T>(IDeviceRepositoryResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Failure : DeviceRepositoryResult
        {
            public override T Accept<T>(IDeviceRepositoryResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}