using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
{
    public class ServiceDefinitionMetaData
    {
        [Required]
        public string Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public ServiceDefinitionType Type { get; set; }
    }
}