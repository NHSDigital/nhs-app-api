using System;
using System.Globalization;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class ConsentRequestToUpdateMapper :
        IMapper<ConsentRequest, DateTimeOffset, UpdateRecordBuilder<TermsAndConditionsRecord>>
    {
        public UpdateRecordBuilder<TermsAndConditionsRecord> Map(ConsentRequest firstSource, DateTimeOffset secondSource)
        {
            var dateOfConsent = secondSource.ToString("s", CultureInfo.InvariantCulture);
            var updates =
                new UpdateRecordBuilder<TermsAndConditionsRecord>()
                    .Set(record => record.ConsentGiven, firstSource.ConsentGiven)
                    .Set(record => record.DateOfConsent, dateOfConsent);

            return updates;
        }
    }
}