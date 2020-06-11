using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordRecallsMapper
    {
        internal void Map(MyRecordResponse myRecordResponse, RecallData recallData)
        {
            if (recallData != null)
            {
                myRecordResponse.Recalls.Data = recallData.Recalls
                    .Select(x => new RecallItem()
                    {
                        RecordDate = x.RecordDate != null
                            ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.RecordDate, out var recordDate)
                                    ? recordDate
                                    : (DateTimeOffset?)null,
                                DatePart = "Unknown"
                            }
                            : null,
                        Name = x.Name,
                        Description = x.Description,
                        Result = x.Result,
                        NextDate = x.NextDate,
                        Status = x.Status
                    })
                    .OrderByDescending(o => o.RecordDate?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.Recalls.HasUndeterminedAccess = !recallData.Recalls.Any();
            }
        }
    }
}