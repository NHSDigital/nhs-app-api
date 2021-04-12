using System;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.Support
{
    public sealed class CalculateAge
    {
        private readonly ICurrentDateTimeProvider _timeProvider;

        public CalculateAge(ICurrentDateTimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public AgeData CalculateAgeInMonthsAndYears(DateTime? dateOfBirth)
        {
            if (dateOfBirth != null)
            {
                return CalculateAgeInMonthsAndYears(dateOfBirth.Value);
            }

            return new AgeData();
        }

        private AgeData CalculateAgeInMonthsAndYears(DateTime dateOfBirth)
        {
            var now = _timeProvider.LocalNow;

            var ageIsLessThanOneYear = dateOfBirth.AddYears(1) > now;
            if (ageIsLessThanOneYear)
            {
                return CalculateAgeInMonths(dateOfBirth, now);
            }

            return CalculateAgeInYears(dateOfBirth, now);
        }

        private static AgeData CalculateAgeInMonths(DateTime dateOfBirth, DateTime now)
        {
            int ageMonths;

            var ageIsLessThanOneMonth = dateOfBirth.AddMonths(1) > now;
            if (ageIsLessThanOneMonth)
            {
                ageMonths = 0;
            }
            else
            {
                // Calculate full months then compare days as
                // 2020-08-31.AddMonths(1) = 2020-09-30
                // So the date comparison alone cannot be relied on
                ageMonths = CalculateFullMonthsSinceDateOfBirth(dateOfBirth, now);

                if (dateOfBirth.Day <= now.Day)
                {
                    ageMonths++;
                }
            }

            return new AgeData
            {
                AgeYears = 0,
                AgeMonths = ageMonths
            };
        }

        private static int CalculateFullMonthsSinceDateOfBirth(DateTime dateOfBirth, DateTime now)
        {
            var fullMonths = 0;
            var targetMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Local);
            while (dateOfBirth.AddMonths(fullMonths + 1) < targetMonth)
            {
                fullMonths++;
            }

            return fullMonths;
        }

        private static AgeData CalculateAgeInYears(DateTime dateOfBirth, DateTime now)
        {
            var ageYears = (now.Year - dateOfBirth.Year) - 1;
            while (dateOfBirth.AddYears(ageYears + 1) < now)
            {
                ageYears++;
            }

            return new AgeData
            {
                AgeYears = ageYears,
                AgeMonths = 0
            };
        }
    }
}