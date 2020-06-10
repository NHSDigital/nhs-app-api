using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Soap;

namespace NHSOnline.Backend.NominatedPharmacy
{
    internal static class NominatedPharmacyApiObjectResponseExtensions
    {
        internal static string GetPertinentSerialChangeNumber(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPdsResponse()?.PertinentInformation?.PertinentSerialChangeNumber?.Value?.Value;

        internal static string GetDateOfBirth(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPatientPerson()?.BirthTime?.Value;

        internal static string GetFamilyName(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPatientPerson()?.COCTMT000203UK02PartOfWhole?.PartPerson?.Name?.Family;

        internal static IEnumerable<GetNominatedPharmacyTypes.PatientCareProvisionEvent> GetPatientCareProvisionEvents(
            this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response,
            string[] knownPharmacyTypes)
        {
            return response?.GetPatientPerson()?.PlayedOtherProviderPatients
                ?.Select(x => x.SubjectOf?.PatientCareProvisionEvent)
                .Where(y => knownPharmacyTypes.Contains(y?.Code?.Code));
        }

        internal static GetNominatedPharmacyTypes.ConfidentialityCode GetPatientRoleConfidentialityCode(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response?.GetPatientRole()?.ConfidentialityCode;

        internal static string GetNhsNumberReturned(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPatientRole()?.Id?.Extension;

        private static GetNominatedPharmacyTypes.PatientPerson GetPatientPerson(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPatientRole()?.PatientPerson;

        private static GetNominatedPharmacyTypes.PatientRole GetPatientRole(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.GetPdsResponse()?.Subject?.PatientRole;

        private static GetNominatedPharmacyTypes.PDSResponse GetPdsResponse(this NominatedPharmacyApiObjectResponse<GetNominatedPharmacyTypes.QUPAIN000009UK03Response> response)
            => response.Body?.QUPAIN000009UK03?.ControlActEvent?.Subject?.PDSResponse;
    }
}