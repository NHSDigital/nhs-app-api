using System;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class MinimumAgeValidator : IMinimumAgeValidator
    {
        private readonly ConfigurationSettings _settings;

        public MinimumAgeValidator(IOptions<ConfigurationSettings> settings)
        {
            _settings = settings.Value;
        }

        public bool IsValid(DateTime dateOfBirth)
        {
            return dateOfBirth.AddYears(_settings.MinimumAge) < DateTime.Now;
        }
    }
}