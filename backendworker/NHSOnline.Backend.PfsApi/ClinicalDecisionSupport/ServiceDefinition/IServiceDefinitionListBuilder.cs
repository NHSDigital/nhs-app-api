using System.Collections.Generic;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionListBuilder {
        List<ServiceDefinitionCategory> BuildServiceDefinitionList(Bundle bundle);
    }
}