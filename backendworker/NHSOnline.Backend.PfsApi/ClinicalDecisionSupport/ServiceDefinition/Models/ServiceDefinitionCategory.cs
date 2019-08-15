using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
{
    public class ServiceDefinitionCategory
    {
        public string Category { get; }

        public List<ServiceDefinitionItem> Items { get; }
        
        public ServiceDefinitionCategory(string category)
        {
            this.Category = category;
            this.Items = new List<ServiceDefinitionItem>();
        }
    }
}