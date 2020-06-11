using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordMedicalHistoryMapper
    {
        private readonly ILogger _logger;

        public MicrotestMyRecordMedicalHistoryMapper(
            ILogger<MicrotestMyRecordMedicalHistoryMapper> logger)
        {
            _logger = logger;
        }

        internal void Map(MyRecordResponse myRecordResponse, MedicalHistoryData medicalHistoryData)
        {
            if (medicalHistoryData != null)
            {
                var medHistoryItems = new List<MedicalHistoryItem>();

                foreach (var medicalHistory in medicalHistoryData.MedicalHistories)
                {
                    AddMedicalHistoryItemIfValid(medHistoryItems, medicalHistory);
                }

                myRecordResponse.MedicalHistories.Data
                    = medHistoryItems.OrderByDescending(p => p.StartDate?.Value.GetValueOrDefault());

                myRecordResponse.MedicalHistories.HasUndeterminedAccess = !medicalHistoryData.MedicalHistories.Any();
            }
        }

        private void AddMedicalHistoryItemIfValid(List<MedicalHistoryItem> medicalHistoryItems, MedicalHistory medicalHistory)
        {
            if (!string.IsNullOrWhiteSpace(medicalHistory.Rubric))
            {
                MyRecordDate medHistoryStartDate = null;
                var validStartDate = DateTime.TryParse(medicalHistory.StartDate, out var parsedStartDate);

                if (validStartDate)
                {
                    medHistoryStartDate = new MyRecordDate
                    {
                        Value = parsedStartDate,
                        DatePart = "Unknown"
                    };
                }

                medicalHistoryItems.Add(new MedicalHistoryItem
                {
                    StartDate = medHistoryStartDate,
                    Rubric = medicalHistory.Rubric,
                    Description = medicalHistory.Description
                });
            }
            else
            {
                _logger.LogInformation("This item will not be mapped as no valid rubric found in MyRecord Medical History from Microtest.");
            }
        }
    }
}