using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    internal class RepositoryGetInfoRecordResultVisitor : IRepositoryFindResultVisitor<UserAndInfo, GetInfoResult>
    {
        public GetInfoResult Visit(RepositoryFindResult<UserAndInfo>.NotFound result)
        {
            return new GetInfoResult.NotFound();
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