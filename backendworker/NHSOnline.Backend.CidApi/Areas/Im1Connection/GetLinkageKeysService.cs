using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public class GetLinkageKeysService : IGetLinkageKeysService
    {
        private readonly ILogger<GetLinkageKeysService> _logger;
        private readonly IAuditor _auditor;

        public GetLinkageKeysService(
            ILogger<GetLinkageKeysService> logger,
            IAuditor auditor)
        {
            _logger = logger;
            _auditor = auditor;
        }

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest request, IGpSystem gpSystem)
        {
            try
            {
                var linkageService = gpSystem.GetLinkageService();

                await _auditor.PreOperationAuditRegistrationEvent(request.NhsNumber, gpSystem.Supplier,
                    AuditingOperations.GetLinkageDetailsAuditTypeRequest, "Attempting to get linkage details.");

                var result = await linkageService.GetLinkageKey(request);

                if (result is LinkageResult.SuccessfullyRetrieved retrieved)
                {
                    retrieved.Response.OdsCode = request.OdsCode;
                }

                return result;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
