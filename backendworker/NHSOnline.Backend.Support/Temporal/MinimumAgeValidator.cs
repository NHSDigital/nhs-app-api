using System;

namespace NHSOnline.Backend.Support.Temporal
{
    public interface IMinimumAgeValidator
    {
        bool IsValid(DateTime dateOfBirth, int minimumAge);
    }

    public class MinimumAgeValidator : IMinimumAgeValidator
    {
        public bool IsValid(DateTime dateOfBirth, int minimumAge)
        {
            return dateOfBirth.AddYears(minimumAge).Date <= DateTime.Today;
        }
    }
}