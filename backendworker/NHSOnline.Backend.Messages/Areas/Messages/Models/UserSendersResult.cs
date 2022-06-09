namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public abstract class UserSendersResult
    {
        public abstract T Accept<T>(IUserSendersResultVisitor<T> visitor);

        public class Found : UserSendersResult
        {
            public UserSendersResponse Response { get; }

            public Found(UserSendersResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IUserSendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class None : UserSendersResult
        {
            public override T Accept<T>(IUserSendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : UserSendersResult
        {
            public override T Accept<T>(IUserSendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : UserSendersResult
        {
            public override T Accept<T>(IUserSendersResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}