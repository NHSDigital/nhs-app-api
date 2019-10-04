using System.Collections.Generic;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public abstract class GetInfoResult
    {
        public abstract T Accept<T>(IGetInfoResultVisitor<T> visitor);

        public class Found : GetInfoResult
        {
            public UserAndInfo UserInfo { get; }

            public Found(UserAndInfo userInfo)
            {
                UserInfo = userInfo;
            }

            public override T Accept<T>(IGetInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class FoundMultiple : GetInfoResult
        {
            public IEnumerable<string> NhsLoginIds { get; }

            public FoundMultiple(IEnumerable<string> userInfo)
            {
                NhsLoginIds = userInfo;
            }

            public override T Accept<T>(IGetInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : GetInfoResult
        {
            public override T Accept<T>(IGetInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetInfoResult
        {
            public override T Accept<T>(IGetInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetInfoResult
        {
            public override T Accept<T>(IGetInfoResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}