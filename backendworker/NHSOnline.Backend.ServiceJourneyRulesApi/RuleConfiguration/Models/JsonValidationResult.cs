using System.Collections.Generic;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models
{
    public class JsonValidationResult
    {
        public string Json { get; set; }
        public bool IsErrors { get; set; }
        public List<string> Errors { get; set; }

        public JsonValidationResult()
        {
            Errors = new List<string>();
        }
    }
}