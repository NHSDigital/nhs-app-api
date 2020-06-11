using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordMapper : IMicrotestMyRecordMapper
    {
        private readonly MicrotestMyRecordSummaryRecordMapper _summaryRecordMapper;
        private readonly MicrotestMyRecordDetailedRecordMapper _detailedRecordMapper;

        public MicrotestMyRecordMapper(
            MicrotestMyRecordSummaryRecordMapper summaryRecordMapper,
            MicrotestMyRecordDetailedRecordMapper detailedRecordMapper)
        {
            _summaryRecordMapper = summaryRecordMapper;
            _detailedRecordMapper = detailedRecordMapper;
        }

        public MyRecordResponse Map(PatientRecordGetResponse patientRecordGetResponse)
        {
            if (patientRecordGetResponse == null)
            {
                throw new ArgumentNullException(nameof(patientRecordGetResponse));
            }

            var myRecordResponse = new MyRecordResponse();

            _summaryRecordMapper.Map(myRecordResponse, patientRecordGetResponse);
            _detailedRecordMapper.Map(myRecordResponse, patientRecordGetResponse);
            SetHasSummaryRecordAccess(myRecordResponse);
            SetHasDetailedRecordAccess(myRecordResponse);

            return myRecordResponse;
        }

        private static void SetHasSummaryRecordAccess(MyRecordResponse myRecordResponse)
        {
            myRecordResponse.HasSummaryRecordAccess =
                IsAny(myRecordResponse.Allergies.Data) ||
                IsAny(myRecordResponse.Medications.Data.AcuteMedications) ||
                IsAny(myRecordResponse.Medications.Data.CurrentRepeatMedications) ||
                IsAny(myRecordResponse.Medications.Data.DiscontinuedRepeatMedications);
        }

        private static void SetHasDetailedRecordAccess(MyRecordResponse myRecordResponse)
        {
            myRecordResponse.HasDetailedRecordAccess =
                IsAny(myRecordResponse.Immunisations.Data) ||
                IsAny(myRecordResponse.Problems.Data) ||
                IsAny(myRecordResponse.TestResults.Data) ||
                IsAny(myRecordResponse.MedicalHistories.Data) ||
                IsAny(myRecordResponse.Recalls.Data) ||
                IsAny(myRecordResponse.Encounters.Data) ||
                IsAny(myRecordResponse.Referrals.Data);
        }

        private static bool IsAny<T>(IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}