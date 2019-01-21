using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support
{
    public abstract class EnumMapper<TEnum> : IEnumMapper<string, TEnum>
        where TEnum: struct
    {
        private readonly ILogger<TEnum> _logger;
        protected abstract Dictionary<string, TEnum> ToMappingTable { get; }
        protected abstract Dictionary<TEnum, string> FromMappingTable { get; }

        protected EnumMapper(ILogger<TEnum> logger)
        {
            _logger = logger;
        }

        public TEnum To(string source)
        {
            if (!string.IsNullOrEmpty(source) && ToMappingTable.ContainsKey(source))
            {
                return ToMappingTable[source];
            }
            
            _logger.LogWarning($"Unable to map {typeof(TEnum).FullName} value: {source}.  Default mapping used");
            return default(TEnum);
        }
        
        public string From(TEnum source)
        {
            if (FromMappingTable.ContainsKey(source))
            {
                return FromMappingTable[source];
            }
            
            _logger.LogWarning($"Unable to map {typeof(string).FullName} value: {source}.  Default mapping used");
            return default(string);
        }
    }
}