using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.UserInfo.Areas.UserInfo.Models;
using NHSOnline.Backend.UserInfo.Repository;

namespace NHSOnline.Backend.UserInfo.Areas.UserInfo
{
    public class InfoService : IInfoService
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

                var currentUserInfo = new UserAndInfo
                {
                    NhsLoginId = accessToken.Subject,
                    Info = new Info
                    {
                        NhsNumber = userProfile.NhsNumber,
                        OdsCode = userProfile.OdsCode
                    }
                };

                var lastSavedUserInfo = await GetUserInfoRecord(currentUserInfo.NhsLoginId);

                await UpdateSecondaryInfoCollections(lastSavedUserInfo, currentUserInfo);

                var repositoryResult = await _infoRepository.CreateOrUpdatePrimary(currentUserInfo);

                return repositoryResult.Accept(new RepositoryCreateResultVisitor());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Info Posting has failed with exception");
                return new PostInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task UpdateSecondaryInfoCollections(
            UserAndInfo lastSavedUserInfo,UserAndInfo currentUserInfo)
        {
            var tasks = new List<Task>
            {
                DeleteObsoleteNhsNumberRecord(lastSavedUserInfo, currentUserInfo),
                CreateOrUpdateNhsNumberRecord(currentUserInfo),
                DeleteObsoleteOdsCodeRecord(lastSavedUserInfo, currentUserInfo),
                CreateOrUpdateOdsCodeRecord(currentUserInfo)
            };

            await Task.WhenAll(tasks);
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
                _logger.LogError(e, "Info Get has failed with exception");
                return new GetInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task<GetInfoResult> GetInfoByNhsNumber(string nhsNumber)
            => await GetInfoRecords(repo => repo.FindByNhsNumber(nhsNumber));

        public async Task<GetInfoResult> GetInfoByOdsCode(string odsCode)
            => await GetInfoRecords(repo => repo.FindByOdsCode(odsCode));

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
                _logger.LogError(e, "Info Get has failed with exception");
                return new GetInfoResult.InternalServerError();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<UserAndInfo> GetUserInfoRecord(string nhsLoginId)
        {
            var userAndInfo = await GetInfoRecords(repo => repo.FindByNhsLoginId(nhsLoginId));

            return (userAndInfo is GetInfoResult.Found userInfoRecord)
                ? userInfoRecord.UserInfoRecords.FirstOrDefault()
                : null;
        }

        private async Task DeleteObsoleteNhsNumberRecord(UserAndInfo lastSavedUserInfo, UserAndInfo currentUserInfo)
        {
            if (!string.IsNullOrEmpty(lastSavedUserInfo?.Info.NhsNumber) &&
                (lastSavedUserInfo.Info.NhsNumber != currentUserInfo.Info.NhsNumber))
            {
                await _infoRepository.DeleteNhsNumberRecord(lastSavedUserInfo.Info.NhsNumber,
                    lastSavedUserInfo.NhsLoginId);
            }
        }

        private async Task CreateOrUpdateNhsNumberRecord(UserAndInfo currentUserInfo)
        {
            if (!string.IsNullOrEmpty(currentUserInfo.Info.NhsNumber))
            {
                await _infoRepository.CreateOrUpdateNhsNumberRecord(currentUserInfo);
            }
        }

        private async Task DeleteObsoleteOdsCodeRecord(UserAndInfo lastSavedUserInfo, UserAndInfo currentUserInfo)
        {
            if (!string.IsNullOrEmpty(lastSavedUserInfo?.Info.OdsCode) &&
                (lastSavedUserInfo.Info.OdsCode != currentUserInfo.Info.OdsCode))
            {
                await _infoRepository.DeleteOdsCodeRecord(lastSavedUserInfo.Info.OdsCode,
                    lastSavedUserInfo.NhsLoginId);
            }
        }

        private async Task CreateOrUpdateOdsCodeRecord(UserAndInfo currentUserInfo)
        {
            if (!string.IsNullOrEmpty(currentUserInfo.Info.OdsCode))
            {
                await _infoRepository.CreateOrUpdateOdsCodeRecord(currentUserInfo);
            }
        }
    }
}