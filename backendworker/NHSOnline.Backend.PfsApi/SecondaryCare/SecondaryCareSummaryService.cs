using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ISecondaryCareClient _secondaryCareClient;

        public SecondaryCareSummaryService(ISecondaryCareClient secondaryCareClient)
        {
            _secondaryCareClient = secondaryCareClient;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            var summaryResponse = await _secondaryCareClient.GetSummary(userSession);

            if (summaryResponse.HasSuccessResponse)
            {
                if (summaryResponse.Body.Referrals?.Count() > 1)
                {
                    summaryResponse.Body.Referrals =
                        summaryResponse.Body.Referrals.OrderBy(referral => referral.ReferredDateTime);
                }

                return new SecondaryCareSummaryResult.Success(summaryResponse.Body);
            }

            return new SecondaryCareSummaryResult.BadGateway();
        }
    }
}