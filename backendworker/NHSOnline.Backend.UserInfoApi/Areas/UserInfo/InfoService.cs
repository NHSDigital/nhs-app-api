using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.UserInfoApi.Repository;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserInfo
{
    internal class InfoService : IInfoService
    {
        private readonly IInfoRepository _infoRepository;
        private readonly ILogger<InfoController> _logger;

        public InfoService
        (
            IInfoRepository infoRepository,
            ILogger<InfoController> logger
        )
        {
            _infoRepository = infoRepository;
            _logger = logger;
        }

        public async Task<PostInfoResult> Send(AccessToken accessToken, string odsCode)
        {
            try
            {
                _logger.LogEnter();
                var userInfo = new UserAndInfo
                {
                    NhsLoginId = accessToken.Subject,
                    Info = new Info()
                    {
                        NhsNumber = accessToken.NhsNumber,
                        OdsCode = odsCode,
                        BetaTester = false
                    }
                };

                return await _infoRepository.Create(userInfo);
            }
            catch (MongoException e)
            {
                _logger.LogError($"Info Posting has failed with exception: {e}");
                return new PostInfoResult.BadGateway();
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
                if (result != null)
                {
                    return new GetInfoResult.Found(result);
                }

                return new GetInfoResult.NotFound();
            }
            catch (MongoException e)
            {
                _logger.LogError($"Info Get has failed with exception: {e}");
                return new GetInfoResult.BadGateway();
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

        private async Task<GetInfoResult> GetInfoRecords(Func<IInfoRepository, Task<IEnumerable<UserAndInfo>>> find)
        {
            _logger.LogEnter();
            try
            {
                var result = await find.Invoke(_infoRepository);

                if (result != null)
                {
                    return new GetInfoResult.FoundMultiple(result.Select(r=>r.NhsLoginId));
                }

                _logger.LogError("Unexpected response from repository, null object returned");
                return new GetInfoResult.InternalServerError();
            }
            catch (MongoException e)
            {
                _logger.LogError($"Info Get has failed with exception: {e}");
                return new GetInfoResult.BadGateway();
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