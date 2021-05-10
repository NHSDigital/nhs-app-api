using System;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class RemovesSilverIntegrationAttribute : Attribute
    {
        public string AttributeToRemove { get; }

        protected RemovesSilverIntegrationAttribute(string attributeToRemove)
        {
            AttributeToRemove = attributeToRemove;
        }
    }
}
