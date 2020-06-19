using System;
using System.Globalization;

namespace NHSOnline.HttpMocks.Domain
{
    public sealed class PatientAge
    {
        public PatientAge(string dateOfBirth)
        {
            DateOfBirth = DateTime.ParseExact(
                dateOfBirth,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        }

        public DateTime DateOfBirth { get; }

        public string DateOfBirthISO86012004 => DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}