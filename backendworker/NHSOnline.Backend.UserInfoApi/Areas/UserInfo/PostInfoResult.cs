using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public abstract class PostInfoResult
    {
        public abstract T Accept<T>(IInfoResultVisitor<T> visitor);

        public class Created : PostInfoResult
        {
            public Created(UserAndInfo record)
            {
                Record = record;
            }

            public UserAndInfo Record { get; }
            public override T Accept<T>(IInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : PostInfoResult
        {
            public override T Accept<T>(IInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : PostInfoResult
        {
            public override T Accept<T>(IInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}