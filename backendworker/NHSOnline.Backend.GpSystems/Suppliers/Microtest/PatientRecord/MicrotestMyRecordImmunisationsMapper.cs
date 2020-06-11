using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordImmunisationsMapper
    {
        internal void Map(MyRecordResponse myRecordResponse, ImmunisationData immunisationData)
        {
            if (immunisationData != null)
            {
                myRecordResponse.Immunisations.Data = immunisationData.Immunisations
                    .Select(x =>
                    {
                        var item = new ImmunisationItem
                        {
                            Term = x.Description,
                            EffectiveDate = x.Date != null
                                ? new MyRecordDate
                                {
                                    Value = DateTime.TryParse(x.Date, out var effectiveDate)
                                        ? effectiveDate
                                        : (DateTimeOffset?)null,
                                    DatePart = "Unknown"
                                }
                                : null,
                            Status = x.Status
                        };

                        if (x.NextDate != null)
                        {
                            item.NextDate = new MyRecordDateRawString();
                            if (DateTime.TryParse(x.NextDate, out var nextDate))
                            {
                                item.NextDate.Value = nextDate;
                                item.NextDate.DatePart = "Unknown";
                            }
                            else
                            {
                                item.NextDate.RawValue = x.NextDate;
                            }
                        }
                        return item;
                    })
                    .OrderByDescending(o => o.EffectiveDate?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.Immunisations.HasUndeterminedAccess = !immunisationData.Immunisations.Any();
            }
        }
    }
}