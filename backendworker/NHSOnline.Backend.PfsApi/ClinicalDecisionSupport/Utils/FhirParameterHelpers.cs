using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support;
using NhsAppFhir = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Fhir;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public class FhirParameterHelpers : IFhirParameterHelpers
    {
        private readonly ILogger<IFhirParameterHelpers> _logger;

        private const string OdsCodeParameter = "ODSCode";
        private const string OrganizationName = "organization";
        private const string OrganizationResourceType = "Organization";
        private const string RequestIdParameter = "requestId";
        private const string NhsAppServiceDefinitionIdParameter = "nhsAppServiceDefinitionId";
        private const string NhsAppServiceDefinitionTypeParameter = "nhsAppServiceDefinitionType";

        public FhirParameterHelpers(ILogger<IFhirParameterHelpers> logger)
        {
            _logger = logger;
        }

        public Patient CreateFhirPatient(IMapper<DemographicsResponse, OlcDemographics> demographicsOlcMapper,
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

        public Parameters RemoveServiceDefinitionMetadataFromParameters(Parameters parameters, out ServiceDefinitionMetaData metaData)
        {
            metaData = null;

            var nhsAppParameterNames = new List<string>
            {
                NhsAppServiceDefinitionIdParameter, NhsAppServiceDefinitionTypeParameter
            };

            var paramGroups = parameters
                .Parameter
                .GroupBy(p => nhsAppParameterNames.Contains(p?.Name, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(g => g.Key, g => g.ToList());

            parameters.Parameter = paramGroups.GetValueOrDefault(false, new List<Parameters.ParameterComponent>());

            try
            {
                var nhsAppParams = paramGroups[true].ToDictionary(p => p.Name, p => p.Value);

                var idParam = (nhsAppParams[NhsAppServiceDefinitionIdParameter] as FhirString)?.Value;
                var typeParam = (nhsAppParams[NhsAppServiceDefinitionTypeParameter] as FhirString)?.Value;

                if (string.IsNullOrWhiteSpace(idParam))
                {
                    _logger.LogError($"Unable to retrieve {NhsAppServiceDefinitionIdParameter} parameter from {nameof(parameters.Parameter)}");
                    return parameters;
                }

                if (!Enum.TryParse(typeParam, true, out ServiceDefinitionType type))
                {
                    _logger.LogWarning($"Unable to parse service definition type: {typeParam} to {nameof(ServiceDefinitionType)}");
                }

                metaData = new ServiceDefinitionMetaData
                {
                    Id = idParam,
                    Type = type
                };
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, $"Error retrieving service definition meta data from {nameof(parameters.Parameter)}");
            }

            return parameters;
        }

        public NhsAppFhir.Parameters CreateInitialServiceDefinitionEvaluateParameters(string odsCode)
        {
            return new NhsAppFhir.Parameters
            {
                Parameter = new List<NhsAppFhir.Parameter>
                {
                    new NhsAppFhir.Parameter
                    {
                        Name = OrganizationName,
                        Resource = new NhsAppFhir.Resource
                        {
                            ResourceType = OrganizationResourceType,
                            Identifier = new NhsAppFhir.Identifier
                            {
                                Value = odsCode
                            }
                        }
                    }
                }
            };
        }

        public Parameters CreateServiceDefinitionIsValidParameters(string odsCode, string requestId)
        {
            return new Parameters
            {
                Parameter = new List<Parameters.ParameterComponent>
                {
                    new Parameters.ParameterComponent
                    {
                        Name = OdsCodeParameter,
                        Value = new FhirString(odsCode)
                    },
                    new Parameters.ParameterComponent
                    {
                        Name = RequestIdParameter,
                        Value = new FhirString(requestId)
                    }
                }
            };
        }
    }
}