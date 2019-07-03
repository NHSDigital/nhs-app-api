using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public abstract class Journey<TEnum>
        where TEnum: struct, IConvertible
    {
        [JsonConverter(typeof(StringEnumConverter), false)]
        public TEnum? Provider { get; set; }
    }
}