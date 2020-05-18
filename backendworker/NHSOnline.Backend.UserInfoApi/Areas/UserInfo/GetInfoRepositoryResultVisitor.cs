using System.Linq;
using NHSOnline.Backend.Support.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    internal class GetInfoRepositoryResultVisitor : IRepositoryFindResultVisitor<UserAndInfo, GetInfoResult>
    {
        public GetInfoResult Visit(RepositoryFindResult<UserAndInfo>.NotFound result)
        {
            return new GetInfoResult.NotFound();
        }

        public GetInfoResult Visit(RepositoryFindResult<UserAndInfo>.InternalServerError result)
        {
            return new GetInfoResult.InternalServerError();
        }

        public GetInfoResult Visit(RepositoryFindResult<UserAndInfo>.RepositoryError result)
        {
            return new GetInfoResult.BadGateway();
        }

        public GetInfoResult Visit(RepositoryFindResult<UserAndInfo>.Found result)
        {
            return new GetInfoResult.Found(result.Records);
        }
    }
}