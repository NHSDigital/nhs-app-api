using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems
{
    public class KeyAndMessageToEnumMapper<TEnum> where TEnum:struct
    {
        private ILogger _logger;

        private readonly List<Mapping> _keyAndMessageMappings = new List<Mapping>();

        private readonly Dictionary<string, TEnum> _keyMappings = new Dictionary<string, TEnum>();

        private readonly Dictionary<string, TEnum> _messageMappings = new Dictionary<string, TEnum>();

        public KeyAndMessageToEnumMapper<TEnum> Add(string key, string message, TEnum mapping)
        {
            _keyAndMessageMappings.Add(new Mapping(key, message, mapping));
            return this;
        }

        public KeyAndMessageToEnumMapper<TEnum> AddKeyToEnum(string key, TEnum mapping)
        {
            _keyMappings. Add(key,  mapping);
            return this;
        }

        public KeyAndMessageToEnumMapper<TEnum> AddMessageToEnum(string message, TEnum mapping)
        {
            _messageMappings.Add(message, mapping);
            return this;
        }

        public TEnum? Map(ILogger logger, string key, params string[] messages)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _logger = logger;

            if (messages.Any() && messages.Any(message => !string.IsNullOrWhiteSpace(message)))
            {
                return messages
                    .Select(message => MapKeyAndMessage(key, message.Trim()))
                    .FirstOrDefault(mapping => mapping != null);
            }

            return MapKeyAndMessage(key, null);
        }

        private TEnum? MapKeyAndMessage(string key, string message)
        {
            var keyAndMessageMapping = MapKeyAndMessageToEnum(key, message);
            if (keyAndMessageMapping != null)
            {
                return keyAndMessageMapping.Value;
            }

            var keyMapping = MapKeyToEnum(key);
            if (keyMapping != null)
            {
                return keyMapping.Value;
            }

            return MapMessageToEnum(message);
        }

        private TEnum? MapKeyAndMessageToEnum(string key, string message)
        {
            var foundMappings = _keyAndMessageMappings
                .Where(mapping =>
                    KeyMatches(mapping, key) &&
                    MessageMatches(mapping, message)).Select(mapping => mapping.MappedValue);

            if (foundMappings.Any())
            {
                _logger.LogInformation($"Mapping found with Key '{key}' and Message '{message}'");
                return foundMappings.First();
            }

            _logger.LogInformation($"No mapping found with Key '{key}' and Message '{message}'");
            return null;
        }

        private TEnum? MapKeyToEnum(string key)
        {
            var keyMappings = _keyMappings.Where(mapping => string.Equals(mapping.Key, key, StringComparison.OrdinalIgnoreCase))
                .Select(mapping => mapping.Value);

            if (keyMappings.Any())
            {
                _logger.LogInformation($"Mapping found with Key '{key}'");
                return keyMappings.First();
            }

            _logger.LogInformation($"No mapping found with Key '{key}'");
            return null;
        }

        private TEnum? MapMessageToEnum(string message)
        {
            if (message != null)
            {
                var messageMappings = _messageMappings
                    .Where(mapping => MessageMatches(mapping, message)).Select(mapping => mapping.Value);
                if (messageMappings.Any())
                {
                    _logger.LogInformation($"Mapping found with Message '{message}'");
                    return messageMappings.First();
                }
                _logger.LogInformation($"No mapping found with Message '{message}'");
            }

            return null;
        }

        private static bool KeyMatches(Mapping mapping, string key)
        {
            return string.Equals(mapping.Key, key, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MessageMatches(Mapping mapping, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message.Contains(mapping.Message, StringComparison.OrdinalIgnoreCase);
            }

            return string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(mapping?.Message);
        }

        private static bool MessageMatches(KeyValuePair<string, TEnum> mapping, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private class Mapping
        {
            public Mapping(string key, string message, TEnum mapping)
            {
                Key = key;
                Message = message;
                MappedValue = mapping;
            }

            public string Key { get;  }
            public string Message { get;  }
            public TEnum MappedValue { get; }
        }
    }
}
