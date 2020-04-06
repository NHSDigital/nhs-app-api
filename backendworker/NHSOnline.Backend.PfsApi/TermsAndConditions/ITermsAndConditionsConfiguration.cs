using System;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsConfiguration
    {
        DateTimeOffset EffectiveDate { get; }
    }
}
