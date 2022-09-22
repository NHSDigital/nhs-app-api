using System.Threading.Tasks;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.NHSApim;
using NHSOnline.Backend.PfsApi.NHSApim.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareApimService : ISecondaryCareApimService
    {
        private readonly INhsApimClient _nhsApimClient;
        private readonly IAuditor _auditor;

        public SecondaryCareApimService(INhsApimClient nhsApimClient, IAuditor auditor)
        {
            _nhsApimClient = nhsApimClient;
            _auditor = auditor;
        }

        public async Task<(NhsApimAuthResponse<ApimAccessToken> authToken, bool isSuccess)> GetAuthToken(P9UserSession userSession, string operation)
        {
            var authToken = await _nhsApimClient.GetAuthToken(userSession.CitizenIdUserSession.NhsLoginIdToken);

            if (!authToken.HasSuccessResponse)
            {
                await _auditor.PostOperationAudit(
                    operation,
                    $"Failed to get Auth token - response code: {authToken.StatusCode}");

                return (null, false);
            }

            return (authToken, true);
        }
    }
}