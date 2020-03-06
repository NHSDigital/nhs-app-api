using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class SilverIntegrations : ICloneable<SilverIntegrations>
    {
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<SecondaryAppointmentProvider> SecondaryAppointments { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<MessagesProvider> Messages { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<ConsultationsProvider> Consultations { get; set; }

        public SilverIntegrations Clone()
        {
            var silverIntegrations = MemberwiseClone() as SilverIntegrations;

            silverIntegrations.SecondaryAppointments = SecondaryAppointments.ToList();
            silverIntegrations.Messages = Messages.ToList();
            silverIntegrations.Consultations = Consultations.ToList();
            
            return silverIntegrations;
        }
    }
}