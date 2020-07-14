using System;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.HttpMocks.Emis
{
    public sealed class EmisCreateSessionDefaultBehaviour : IEmisCreateSessionBehaviour
    {
        public IActionResult Behave(EmisPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            var userPatientLinks = new[]
            {
                new
                {
                    name.Title,
                    FirstName = name.GivenName,
                    Surname = name.FamilyName,
                    patient.UserPatientLinkToken,
                    patient.OdsCode,
                    NationalPracticeCode = patient.OdsCode,
                    AssociationType = "Self"
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