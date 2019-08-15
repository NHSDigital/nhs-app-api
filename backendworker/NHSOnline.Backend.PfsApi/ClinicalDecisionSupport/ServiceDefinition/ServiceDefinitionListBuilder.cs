using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public class ServiceDefinitionListBuilder : IServiceDefinitionListBuilder {

        public List<ServiceDefinitionCategory> BuildServiceDefinitionList(Bundle bundle)
        {
            var serviceDefinitionCategories = new List<ServiceDefinitionCategory>();

            var serviceDefinitionEntries = bundle.Entry
                .Where(e => e.Resource.ResourceType == ResourceType.ServiceDefinition)
                .Select(e => e.Resource as Hl7.Fhir.Model.ServiceDefinition).ToList();
       
            foreach (var entry in serviceDefinitionEntries) {
                var categoryName = entry.Topic.First().Coding.First().Display;

                var serviceDefinitionCategory = serviceDefinitionCategories
                    .Where(a => a.Category.Equals(categoryName, System.StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (serviceDefinitionCategory == null) {
                    serviceDefinitionCategory = new ServiceDefinitionCategory(categoryName);
                    serviceDefinitionCategories.Add(serviceDefinitionCategory);
                }

                serviceDefinitionCategory.Items.AddRange(BuildServiceDefinitionItems(entry.Id, entry.Title, entry.UseContext));
            }

            var returnValue = new List<ServiceDefinitionCategory>();

            foreach (var category in serviceDefinitionCategories) {
                var newCategory = new ServiceDefinitionCategory(category.Category);
                newCategory.Items.AddRange(category.Items.OrderBy(a => a.Title));
                returnValue.Add(newCategory);
            }

            return returnValue.OrderBy(sd => sd.Category).ToList();
        }

        private static List<ServiceDefinitionItem> BuildServiceDefinitionItems(string id, string title, List<UsageContext> useContextList) 
        {
            
            var serviceDefinitionItems = new List<ServiceDefinitionItem>();

            serviceDefinitionItems.Add(new ServiceDefinitionItem(id, title));

            foreach (var useContext in useContextList) {
                serviceDefinitionItems.Add(new ServiceDefinitionItem(id, useContext.Code.Display));
            }

            return serviceDefinitionItems.OrderBy(item => item.Title).ToList();
        }
    }
}
