using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordEncountersMapper
    {
        private readonly ILogger _logger;

        public MicrotestMyRecordEncountersMapper(
            ILogger<MicrotestMyRecordEncountersMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, EncounterData encountersData)
        {
            if (encountersData != null)
            {
                var encounters = encountersData.Encounters.Where(HasDescription).ToList();

                var removedCount = encountersData.Count - encounters.Count;
                if (removedCount != 0)
                {
                    _logger.LogInformation(
                        $"{removedCount} items filtered out of encounters due to value stored in Description field.");
                }

                myRecordResponse.Encounters.Data = encounters
                    .Select(x => new EncounterItem
                    {
                        Description = x.Description,
                        Value = x.Value,
                        Unit = x.Unit,
                        RecordedOn = x.RecordedOn != null ? new MyRecordDate
                            {
                                Value = DateTime.TryParse(x.RecordedOn, out var encounterDate)
                                    ? encounterDate
                                    : (DateTimeOffset?)null,
                                DatePart = "Unknown"
                            }
                            : null
                    })
                    .OrderByDescending(o => o.RecordedOn?.Value.GetValueOrDefault())
                    .ToList();

                myRecordResponse.Encounters.HasUndeterminedAccess = !encountersData.Encounters.Any();
            }

            bool HasDescription(Encounter encounter) => !string.IsNullOrEmpty(encounter.Description);
        }
    }
}