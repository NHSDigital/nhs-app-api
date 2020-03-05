using System;

namespace NHSOnline.Backend.Support
{
    public static class CalculateAge
    {
        private const double AverageNumberOfDaysInMonth = 30.4;
        
        public static AgeData CalculateAgeInMonthsAndYears(DateTime? dateOfBirth)
        {
            if (dateOfBirth != null)
            {
                DateTime now = DateTime.Now;

                int ageMonths = 0;
                int ageInYears = now.Year - dateOfBirth.Value.Year -
                                 (dateOfBirth.Value.DayOfYear < now.DayOfYear ? 0 : 1);

                if (ageInYears == 0)
                {
                    TimeSpan timeDifference = now - dateOfBirth.Value;
                    ageMonths = Convert.ToInt32(timeDifference.Days / AverageNumberOfDaysInMonth);
                }

                var calculatedAge = new AgeData
                {
                    AgeMonths = ageMonths,
                    AgeYears = ageInYears
                };

                return calculatedAge;
            }

            return new AgeData();
        }
    }
}