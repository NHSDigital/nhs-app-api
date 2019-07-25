using Hl7.Fhir.Model;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public interface ICreateFhirParameter
    {
        Patient CreatePatientFhir(IMapper<DemographicsResponse, OlcDemographics> demographicsOlcMapper,
            DemographicsResult.Success demographicsResult);
    }
}