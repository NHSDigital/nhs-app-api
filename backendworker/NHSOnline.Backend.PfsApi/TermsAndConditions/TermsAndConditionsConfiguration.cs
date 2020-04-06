using System;
using System.Globalization;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class TermsAndConditionsConfiguration : ITermsAndConditionsConfiguration
    {
        public DateTimeOffset EffectiveDate { get; }

        internal TermsAndConditionsConfiguration(string effectiveDate)
        {
            EffectiveDate =  DateTimeOffset.Parse(effectiveDate, CultureInfo.InvariantCulture);
        }
    }
}
