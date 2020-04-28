using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Fhir
{
    [SuppressMessage("Microsoft.Design", "CA1724", Justification = "Namespace is sufficiently different")]
    public class Parameters
    {
        public string ResourceType { get; set; } = "Parameters";
        public List<Parameter> Parameter { get; set; }
    }
}