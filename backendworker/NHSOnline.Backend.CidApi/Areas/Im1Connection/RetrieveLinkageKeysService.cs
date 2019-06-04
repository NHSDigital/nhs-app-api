using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public class RetrieveLinkageKeysService : IRetrieveLinkageKeysService
    {
        private readonly IGetLinkageKeysService _getLinkageKeysService;
        private readonly ICreateLinkageKeysService _createLinkageKeysService;
        private readonly ILogger<RetrieveLinkageKeysService> _logger;
        
        public RetrieveLinkageKeysService(
            ILogger<RetrieveLinkageKeysService> logger,
            IGetLinkageKeysService getLinkageKeysService,
            ICreateLinkageKeysService createLinkageKeysService)
        {
            _logger = logger;
            _getLinkageKeysService = getLinkageKeysService;
            _createLinkageKeysService = createLinkageKeysService;
        }
        
        public async Task<LinkageResult> RetrieveLinkageKey(RetrieveLinkageKeysRequest model, IGpSystem gpSystem)
        {
            try
            {
                _logger.LogEnter();

                var getLinkageRequest = BuildGetLinkageRequest(model);

                _logger.LogInformation("Getting LinkageKey from GP supplier.");
                var getLinkageKey = await _getLinkageKeysService.GetLinkageKey(getLinkageRequest, gpSystem);
                
                if (IsGetLinkageSuccess(getLinkageKey) || !ShouldCreateLinkageKey(getLinkageKey))
                {
                    return getLinkageKey;
                }
                _logger.LogInformation("Existing LinkageKey not found. Response allows a linkage key to be created.");
                var createLinkageRequest = BuildCreateLinkageRequest(model);

                var createLinkageKey = await _createLinkageKeysService.CreateLinkageKey(createLinkageRequest, gpSystem);

                return createLinkageKey;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private bool IsGetLinkageSuccess(LinkageResult result)
        {
            if (result is LinkageResult.SuccessfullyRetrievedAlreadyExists ||
                result is LinkageResult.SuccessfullyRetrieved)
            {
                _logger.LogInformation("Linkage key was successfully returned from the GP system");
                return true;
            }

            _logger.LogWarning("Failed to retrieve linkage keys from GP supplier");


            return false;
        }

        private static bool ShouldCreateLinkageKey(LinkageResult result)
        {
            return result is LinkageResult.NotFound || GetLinkageResultAllowsCreateLinkage(result);
        }

        private static bool GetLinkageResultAllowsCreateLinkage(LinkageResult result)
        {
            if (result is LinkageResult.ErrorCase errorCase)
            {
                return ContinueToCreateLinkageKeyErrorCodes.Contains(errorCase.ErrorCode);
            }

            return false;
        }

        private static readonly Im1ConnectionErrorCodes.Code[] ContinueToCreateLinkageKeyErrorCodes =
        {
            Im1ConnectionErrorCodes.Code.UnknownError,
            Im1ConnectionErrorCodes.Code.NoSelfAssociatedUserExistWithThisPatient,
            Im1ConnectionErrorCodes.Code.PatientNotRegisteredAtThisPractice,
            Im1ConnectionErrorCodes.Code.NoApiKeyAssociatedWithNhsNumber,
            Im1ConnectionErrorCodes.Code.NoUserAssociatedWithNhsNumber,
            Im1ConnectionErrorCodes.Code.PatientRecordNotFound
        };

        private static CreateLinkageRequest BuildCreateLinkageRequest(RetrieveLinkageKeysRequest model)
        {
            return new CreateLinkageRequest
            {
                NhsNumber = model.NhsNumber,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                OdsCode = model.OdsCode,
                IdentityToken = model.IdentityToken,
                EmailAddress = model.EmailAddress
            };
        }

        private static GetLinkageRequest BuildGetLinkageRequest(RetrieveLinkageKeysRequest model)
        {
            return new GetLinkageRequest
            {
                NhsNumber = model.NhsNumber,
                Surname = model.Surname,
                DateOfBirth = model.DateOfBirth,
                OdsCode = model.OdsCode,
                IdentityToken = model.IdentityToken
            };
        }
    }
}
