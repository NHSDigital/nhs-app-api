namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class RegisterDeviceResult
    {
        public abstract T Accept<T>(IRegisterDeviceResultVisitor<T> visitor);

        public class Created : RegisterDeviceResult
        {
            public UserDevice UserDevice { get; }

            public Created(UserDevice userDevice)
            {
                UserDevice = userDevice;
            }

            public override T Accept<T>(IRegisterDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : RegisterDeviceResult
        {
            public override T Accept<T>(IRegisterDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RegisterDeviceResult
        {
            public override T Accept<T>(IRegisterDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}