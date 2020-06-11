using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordReferralsMapper
    {
        private readonly ILogger _logger;

        public MicrotestMyRecordReferralsMapper(
            ILogger<MicrotestMyRecordReferralsMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, ReferralData referralsData)
        {
            if (referralsData != null)
            {
                var referrals = referralsData.Referrals.Where(o => !string.IsNullOrEmpty(o.Description)).ToList();

                var removedCount = referralsData.Count - referrals.Count;
                if (removedCount != 0)
                {
                    _logger.LogInformation(
                        $"{removedCount} items filtered out of Referrals due to value stored in Description field.");
                }

                myRecordResponse.Referrals.Data = referrals
                    .Select(x => new ReferralItem
                    {
                        Description = x.Description,
                        Speciality = x.Speciality,
                        Ubrn = x.Ubrn,
                        RecordDate = x.RecordDate != null ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.RecordDate, out var referralDate)
                                    ? referralDate
                                    : (DateTimeOffset?)null,
                                DatePart = "Unknown"
                            }
                            : null
                    })
                    .OrderByDescending(o => o.RecordDate?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.Referrals.HasUndeterminedAccess = !referralsData.Referrals.Any();
            }
        }
    }
}