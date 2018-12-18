using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support
{
    public abstract class EnumMapper<TEnum> : IMapper<string, TEnum>
        where TEnum: struct
    {
        private readonly ILogger<TEnum> _logger;
        protected abstract Dictionary<string, TEnum> MappingTable { get; }

        protected EnumMapper(ILogger<TEnum> logger)
        {
            _logger = logger;
        }

        public TEnum Map(string source)
        {
            if (!string.IsNullOrEmpty(source) && MappingTable.ContainsKey(source))
            {
                return MappingTable[source];
            }
            
            _logger.LogWarning($"Unable to map {typeof(TEnum).FullName} value: {source}.  Default mapping used");
            return default(TEnum);
        }
    }
}