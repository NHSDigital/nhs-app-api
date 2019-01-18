using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Linkage
{
    public class MicrotestLinkageService : ILinkageService
    {
        public const string TemporaryAccountId = "MICROTEST_ACCOUNT_ID";
        public const string TemporaryLinkageKey = "MICROTEST_LINKAGE_KEY";

        public async Task<LinkageResult> GetLinkageKey(GetLinkageRequest getLinkageRequest)
        {
            var linkage = await Task.FromResult(new LinkageResponse
            {
                AccountId = TemporaryAccountId,
                LinkageKey = TemporaryLinkageKey,
                OdsCode = getLinkageRequest.OdsCode
            });
            return new LinkageResult.SuccessfullyRetrieved(linkage);
        }

        public async Task<LinkageResult> CreateLinkageKey(CreateLinkageRequest createLinkageRequest)
        {
            return await Task.FromResult(new LinkageResult.NotFoundErrorCreatingNhsUser());
        }
    }
}