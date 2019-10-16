using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.LinkedAccounts
{
    public class EmisLinkedAccountsService : ILinkedAccountsService
    {
        private readonly ILogger<EmisLinkedAccountsService> _logger;
        private readonly IEmisDemographicsService _demographicsService;

        public EmisLinkedAccountsService(ILogger<EmisLinkedAccountsService> logger, IEmisDemographicsService demographicsService)
        {
            _logger = logger;
            _demographicsService = demographicsService;
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            GetLinkedAccountsResponse response = new GetLinkedAccountsResponse();

            if (gpUserSession.HasLinkedAccounts)
            {
                var emisUserSession = (EmisUserSession)gpUserSession;

                var tasks = new Dictionary<Guid, Task<DemographicsResult>>();

                foreach (var user in emisUserSession.ProxyPatients)
                {
                    // Using SessionId and EndUserSessionId of the logged in user
                    // but the UserPatientLinkToken of the user they are acting
                    // on behalf of.
                    var demographicsTask = _demographicsService.GetDemographics(new EmisUserSession
                    {
                        SessionId = emisUserSession.SessionId,
                        EndUserSessionId = emisUserSession.EndUserSessionId,
                        UserPatientLinkToken = user.UserPatientLinkToken,
                    });

                    tasks.Add(user.Id, demographicsTask);
                }

                await Task.WhenAll(tasks.Select(x => x.Value));

                var successResults = tasks
                    .Where(x => x.Value.Result is DemographicsResult.Success)
                    .ToList();
                
                response.LinkedAccounts = successResults.Select(x => {
                    var demographics = (DemographicsResult.Success)x.Value.Result;

                    return new LinkedAccount
                    {
                        Id = x.Key,
                        Name = demographics.Response.PatientName,
                        DateOfBirth = demographics.Response.DateOfBirth,
                    };
                });
            }

            return new LinkedAccountsResult.Success(response);
        }
    }
}
