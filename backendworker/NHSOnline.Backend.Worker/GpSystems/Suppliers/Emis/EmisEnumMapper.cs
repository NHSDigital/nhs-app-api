using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.Areas.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisEnumMapper : IEmisEnumMapper
    {
        private readonly ILogger<EmisEnumMapper> _logger;

        private static readonly IDictionary<string, Necessity> NecessityMap =
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
            if (!string.IsNullOrEmpty(emisNecessity) && NecessityMap.ContainsKey(emisNecessity))
            {
                return NecessityMap[emisNecessity];
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
        
        private static readonly IDictionary<string, Channel> SlotTypeStatusMap =
            new Dictionary<string, Channel>(StringComparer.OrdinalIgnoreCase)
            {
                { "Unknown", Channel.Unknown },
                { "Practice", Channel.Unknown },
                { "Telephone", Channel.Telephone },
                { "Visit", Channel.Unknown },
                { "Video", Channel.Unknown }
            };
        
        public Channel MapSlotTypeStatus(string emisSlotTypeStatus, Channel? defaultValue)
        {
            if (!string.IsNullOrEmpty(emisSlotTypeStatus) && SlotTypeStatusMap.ContainsKey(emisSlotTypeStatus))
            {
                return SlotTypeStatusMap[emisSlotTypeStatus];
            }

            if (defaultValue.HasValue)
            {
                _logger.LogWarning($"Unable to map Emis SlotTypeStatus value: {emisSlotTypeStatus}.  Default mapping used");
                return defaultValue.Value;
            }

            var errorMessage = $"Unable to map Emis SlotTypeStatus value: {emisSlotTypeStatus} and no default mapping provided";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage, nameof(emisSlotTypeStatus));
        }
    }
}
