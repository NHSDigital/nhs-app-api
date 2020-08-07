namespace NHSOnline.Backend.UsersApi.Repository
{
    public abstract class UpdateDeviceResult
    {
        public class InternalServerError : UpdateDeviceResult
        {
            public override T Accept<T>(IUpdateDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public abstract T Accept<T>(IUpdateDeviceResultVisitor<T> visitor);

        public class Updated : UpdateDeviceResult
        {
            public override T Accept<T>(IUpdateDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : UpdateDeviceResult
        {
            public override T Accept<T>(IUpdateDeviceResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}