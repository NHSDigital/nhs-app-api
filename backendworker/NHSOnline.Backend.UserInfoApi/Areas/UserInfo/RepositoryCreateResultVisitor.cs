using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<UserAndInfo, PostInfoResult>
    {
        public PostInfoResult Visit(RepositoryCreateResult<UserAndInfo>.Created result)
        {
            return new PostInfoResult.Created(result.Record);
        }
        
        public PostInfoResult Visit(RepositoryCreateResult<UserAndInfo>.InternalServerError result)
        {
            return new PostInfoResult.InternalServerError();
        }

        public PostInfoResult Visit(RepositoryCreateResult<UserAndInfo>.RepositoryError result)
        {
            return new PostInfoResult.BadGateway();
        }
    }
}