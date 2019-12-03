using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules
{
    public class LinkedAccountsConfigResultVisitor : ILinkedAccountsConfigResultVisitor<IActionResult>
    {
        public IActionResult Visit(LinkedAccountsConfigResult.Success result)
        {
            var response = new LinkedAccountsConfigResponse
            {
                Id = result.PatientId,
                HasLinkedAccounts = result.SessionSettings.ProxyEnabled && result.LinkedAccountsBreakdownSummary.ValidAccounts.Any(),
                LinkedAccounts = result.SessionSettings.ProxyEnabled
                    ? result.LinkedAccountsBreakdownSummary.ValidAccounts
                    : Enumerable.Empty<LinkedAccount>(),
            };

            return new OkObjectResult(response);
        }
    }
}