using System.Globalization;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Tpp.Models;

namespace NHSOnline.HttpMocks.Tpp
{
    public static class TppDefaults
    {
        public static Person DefaultTppPerson(TppPatient patient)
        {
            var name = patient.PersonalDetails.Name;
            return new Person
            {
                PatientId = patient.Id,
                DateOfBirth =
                    patient.PersonalDetails.Age.DateOfBirth.ToString("yyyy-MM-dd'T'HH:mm:ss.f'Z'",
                        CultureInfo.InvariantCulture),
                Gender = patient.PersonalDetails.Gender.ToString(),
                NationalId = new NationalId
                {
                    Type = "NHS",
                    Value = patient.NhsNumber.FormattedStringValue
                },
                PersonName = new PersonName
                {
                    Name = $"{name.Title} {name.GivenName} {name.FamilyName}"
                }
            };
        }
    }
}