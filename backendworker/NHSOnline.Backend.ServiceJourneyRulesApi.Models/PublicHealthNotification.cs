using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHSOnline.Backend.Support;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class PublicHealthNotification : ICloneable<PublicHealthNotification>
    {
        public string Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), false)]
        public NotificationType? Type { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), false)]
        public NotificationUrgency? Urgency { get; set; }
        
        public string Title { get; set; }
        
        [YamlMember(ScalarStyle = ScalarStyle.Literal)]
        public string Body { get; set; }

        public PublicHealthNotification Clone() => new PublicHealthNotification
        {
            Id = Id,
            Type = Type,
            Urgency = Urgency,
            Title = Title,
            Body = Body
        };
    }
}