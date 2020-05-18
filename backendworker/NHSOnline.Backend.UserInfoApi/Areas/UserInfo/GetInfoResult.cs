using System.Collections.Generic;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    public abstract class GetInfoResult
    {
        public abstract T Accept<T>(IGetInfoResultVisitor<T> visitor);

        public class Found : GetInfoResult
        {
            public IEnumerable<UserAndInfo> UserInfoRecords { get; }

            public Found(IEnumerable<UserAndInfo> userInfo)
            {
                UserInfoRecords = userInfo;
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