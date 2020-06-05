using System;
using System.Globalization;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class AnalyticsCookieAcceptanceToUpdateMapper :
        IMapper<AnalyticsCookieAcceptance, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>
    {
        public UpdateRecordBuilder<TermsAndConditionsRecord> Map(AnalyticsCookieAcceptance firstSource, DateTimeOffset secondSource)
        {
            var updates = new UpdateRecordBuilder<TermsAndConditionsRecord>()
                .Set(record => record.AnalyticsCookieAccepted, firstSource.AnalyticsCookieAccepted)
                .Set(record => record.DateOfAnalyticsCookieToggle,
                    secondSource.ToString("s", CultureInfo.InvariantCulture));

            return updates;
        }
    }
}