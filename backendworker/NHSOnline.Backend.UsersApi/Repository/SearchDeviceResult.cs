namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class SearchDeviceResult
    {
        public class Found : SearchDeviceResult
        {
            public UserDevice UserDevice { get; }

            public Found(UserDevice userDevice)
            {
                UserDevice = userDevice;
            }

            public override T Accept<T>(ISearchDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : SearchDeviceResult
        {
            public override T Accept<T>(ISearchDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : SearchDeviceResult
        {
            public override T Accept<T>(ISearchDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : SearchDeviceResult
        {
            public override T Accept<T>(ISearchDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public abstract T Accept<T>(ISearchDeviceResultVisitor<T> visitor);
    }
}