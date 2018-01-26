using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public class EmisNhsNumberProvider : INhsNumberProvider
    {
        private readonly IEmisClient _emisClient;

        public EmisNhsNumberProvider(IEmisClient emisClient)
        {
            _emisClient = emisClient;
        }

        public async Task<IEnumerable<PatientNhsNumber>> GetNhsNumbersAsync(string connectionToken, string odsCode)
        {
            var endUserSessionResponse = await _emisClient.EndUserSessionAsync();
            var endUserSessionId = endUserSessionResponse.EndUserSessionId;
            var sessionsResponse = await _emisClient.SessionsAsync(endUserSessionId, connectionToken, odsCode);
            var userPatientLinkToken = sessionsResponse
                ?.UserPatientLinks
                ?.FirstOrDefault(x => x.AssociationType == AssociationType.Self)
                ?.UserPatientLinkToken;

            if (userPatientLinkToken == null)
            {
                return new PatientNhsNumber[0];
            }

            var demographicsResponse = await _emisClient.DemographicsAsync(userPatientLinkToken, sessionsResponse.SessionId, endUserSessionId);
            var patientIdentifiers = demographicsResponse?.PatientIdentifiers;

            if (patientIdentifiers == null)
            {
                return new PatientNhsNumber[0];
            }

            return patientIdentifiers
                .Where(x => x.IdentifierType == IdentifierType.NhsNumber)
                .Select(x => new PatientNhsNumber { NhsNumber = x.IdentifierValue });
        }
    }
}
