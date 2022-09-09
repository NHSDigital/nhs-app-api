using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisCreateSessionLinkedProfileBehaviour: IEmisCreateSessionLinkedProfileBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            var userPatientLinks = new[]
            {
                new
                {
                    Title = name.Title,
                    FirstName = name.GivenName,
                    Surname = name.FamilyName,
                    UserPatientLinkToken = patient.UserPatientLinkToken,
                    OdsCode = patient.OdsCode,
                    NationalPracticeCode = patient.OdsCode,
                    AssociationType = "Self",
                    NhsNumber = patient.NhsNumber.StringValue,
                    PatientActivityContextGuid = patient.PatientActivityContextGuid
                },
                new
                {
                    Title = patient.LinkedProfiles[0].PersonalDetails.Name.Title,
                    FirstName = patient.LinkedProfiles[0].PersonalDetails.Name.GivenName,
                    Surname = patient.LinkedProfiles[0].PersonalDetails.Name.FamilyName,
                    UserPatientLinkToken = patient.LinkedProfiles[0].UserPatientLinkToken,
                    OdsCode = "emis_with_all_silvers",
                    NationalPracticeCode= "emis_with_all_silvers",
                    AssociationType = "Proxy",
                    NhsNumber = patient.LinkedProfiles[0].NhsNumber.StringValue,
                    PatientActivityContextGuid = patient.LinkedProfiles[0].PatientActivityContextGuid
                }
            };

            return new JsonResult(new
            {
                ApplicationLinkLevel = "Linked",
                LastAccessTime = DateTime.Now.AddDays(-1),
                UserPatientLinks = userPatientLinks,
                patient.SessionId,
                name.Title,
                FirstName = name.GivenName,
                Surname = name.FamilyName
            });
        }
    }
}