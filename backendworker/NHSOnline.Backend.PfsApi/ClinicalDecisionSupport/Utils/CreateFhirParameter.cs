using System.Collections.Generic;
using Hl7.Fhir.Model;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public class CreateFhirParameter : ICreateFhirParameter
    {
        public Patient CreatePatientFhir(IMapper<DemographicsResponse, OlcDemographics> demographicsOlcMapper,
            DemographicsResult.Success demographicsResult)
        {
            var response = demographicsOlcMapper.Map(demographicsResult.Response);

            var identifierList = new List<Identifier>();
            var identifier = new Identifier
            {
                Value = response.NhsNumber, System = "https://fhir.nhs.uk/Id/nhs-number"
            };
            identifierList.Add(identifier);
            var fhirAddress = new Address { Text = response.AddressFull };
            var addressList = new List<Address> { fhirAddress };

            var patient = new Patient
            {
                Identifier = identifierList,
                BirthDate = response.DateOfBirth.FormatToYYYYMMDD(),
                Address = addressList
            };

            if (response.Name != null)
            {
                var name = new HumanName { Family = response.Name.Surname };
                var givenNames = new List<string> { response.Name.GivenName };
                name.Given = givenNames;
                var humanNames = new List<HumanName> { name };
                patient.Name = humanNames;
            } else if (response.NameFull != null)
            {
                var name = new HumanName { Text = response.NameFull };
                var humanNames = new List<HumanName> { name };
                patient.Name = humanNames;
            }
            return patient;
        }
    }
}