using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.SharedModels;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisEnumMapper : IEmisEnumMapper
    {
        private readonly ILogger<EmisEnumMapper> _logger;

        private static readonly IDictionary<string, Necessity> _necessityMap =
            new Dictionary<string, Necessity>(StringComparer.OrdinalIgnoreCase)
            {
                { "NotRequested", Necessity.NotAllowed },
                { "RequestedOptional", Necessity.Optional },
                { "RequestedMandatory", Necessity.Mandatory }
            };

        public EmisEnumMapper(ILogger<EmisEnumMapper> logger)
        {
            _logger = logger;
        }

        public Necessity MapNecessity(string emisNecessity, Necessity? defaultValue)
        {
            if (!string.IsNullOrEmpty(emisNecessity) && _necessityMap.ContainsKey(emisNecessity))
            {
                return _necessityMap[emisNecessity];
            }

            if (defaultValue.HasValue)
            {
                _logger.LogWarning($"Unable to map Emis neccessity value: {emisNecessity}.  Default mapping used");
                return defaultValue.Value;
            }

            var errorMessage = $"Unable to map Emis neccessity value: {emisNecessity} and no default mapping provided";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage, nameof(emisNecessity));
        }
    }
}
