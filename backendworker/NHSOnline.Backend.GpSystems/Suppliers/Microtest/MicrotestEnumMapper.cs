using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestEnumMapper : IMicrotestEnumMapper
    {
        private readonly ILogger<MicrotestEnumMapper> _logger;

        private static readonly IDictionary<string, Channel> ChannelMappings =
            new Dictionary<string, Channel>(StringComparer.OrdinalIgnoreCase)
            {
                { "Other", Channel.Unknown },
                { "Surgery", Channel.Unknown },
                { "Telephone", Channel.Telephone },
                { "Visit", Channel.Unknown }
            };

        public MicrotestEnumMapper(ILogger<MicrotestEnumMapper> logger)
        {
            _logger = logger;
        }

        public Channel MapChannel(string microtestChannel, Channel? defaultValue)
        {
            if (!string.IsNullOrEmpty(microtestChannel) && ChannelMappings.ContainsKey(microtestChannel))
            {
                return ChannelMappings[microtestChannel];
            }

            if (defaultValue.HasValue)
            {
                _logger.LogWarning($"Unable to map Microtest Channel value: {microtestChannel}. Default mapping used");
                return defaultValue.Value;
            }

            var errorMessage = $"Unable to map Microtest Channel value: {microtestChannel} and no default mapping provided";
            _logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage, nameof(microtestChannel));
        }
    }
}
