using System;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class MinimumAgeValidator : IMinimumAgeValidator
    {
        public bool IsValid(DateTime dateOfBirth, int minimumAge)
        {
            return dateOfBirth.AddYears(minimumAge).Date <= DateTime.Today;
        }
    }
}