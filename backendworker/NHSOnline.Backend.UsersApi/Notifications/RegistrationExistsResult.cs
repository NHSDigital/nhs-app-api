namespace NHSOnline.Backend.UsersApi.Notifications
{
    public abstract class RegistrationExistsResult
    {
        public abstract T Accept<T>(IRegistrationExistsResultVisitor<T> visitor);

        public class Found : RegistrationExistsResult
        {
            public override T Accept<T>(IRegistrationExistsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : RegistrationExistsResult
        {
            public override T Accept<T>(IRegistrationExistsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : RegistrationExistsResult
        {
            public override T Accept<T>(IRegistrationExistsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : RegistrationExistsResult
        {
            public override T Accept<T>(IRegistrationExistsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}