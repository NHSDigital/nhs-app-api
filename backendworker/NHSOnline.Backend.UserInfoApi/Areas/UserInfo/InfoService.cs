using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfoApi.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    internal class InfoService : IInfoService
    {
        private readonly IInfoRepository _infoRepository;
        private readonly ILogger<InfoService> _logger;

        public InfoService(IInfoRepository infoRepository, ILogger<InfoService> logger)
        {
            _infoRepository = infoRepository;
            _logger = logger;
        }

        public async Task<PostInfoResult> Send(AccessToken accessToken, InfoUserProfile userProfile)
        {
            try
            {
                _logger.LogEnter();
                var userInfo = new UserAndInfo
                {
                    NhsLoginId = accessToken.Subject,
                    Info = new Info
                    {
                        NhsNumber = userProfile.NhsNumber,
                        OdsCode = userProfile.OdsCode,
                        BetaTester = false
                    }
                };

                var repositoryResult =  await _infoRepository.Create(userInfo);

                return repositoryResult.Accept(new RepositoryCreateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Info Posting has failed with exception: {e}");
                return new PostInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetInfoResult> GetInfo(AccessToken accessToken)
        {
            _logger.LogEnter();

            try
            {
                var result = await _infoRepository.FindByNhsLoginId(accessToken.Subject);
                return result.Accept(new RepositoryGetInfoRecordResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Info Get has failed with exception: {e}");
                return new GetInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetInfoResult> GetInfoByNhsNumber(string nhsNumber)
        {
            return await GetInfoRecords(repo => repo.FindByNhsNumber(nhsNumber));
        }

        public async Task<GetInfoResult> GetInfoByOdsCode(string odsCode)
        {
            return await GetInfoRecords(repo => repo.FindByOdsCode(odsCode));
        }

        private async Task<GetInfoResult> GetInfoRecords(Func<IInfoRepository, Task<RepositoryFindResult<UserAndInfo>>> find)
        {
            _logger.LogEnter();
            try
            {
                var result = await find.Invoke(_infoRepository);
                return result.Accept(new RepositoryGetAllInfoRecordsResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError($"Info Get has failed with exception: {e}");
                return new GetInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}