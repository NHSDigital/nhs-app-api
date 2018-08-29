using System;
using System.Globalization;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Areas.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Temporal;

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