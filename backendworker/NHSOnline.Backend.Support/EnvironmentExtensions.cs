using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Support
{
    public static class EnvironmentExtensions
    {
        public static string GetOrThrow(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrEmpty(value))
            {
                throw new ConfigurationNotValidException(key);
            }

            return value;
        }
    }
}