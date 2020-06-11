using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord
{
    internal sealed class MicrotestMyRecordDetailedRecordMapper
    {
        private readonly MicrotestMyRecordImmunisationsMapper _immunisationsMapper;
        private readonly MicrotestMyRecordProblemsMapper _problemsMapper;
        private readonly MicrotestMyRecordTestResultsMapper _testResultsMapper;
        private readonly MicrotestMyRecordMedicalHistoryMapper _medicalHistoryMapper;
        private readonly MicrotestMyRecordRecallsMapper _recallsMapper;
        private readonly MicrotestMyRecordEncountersMapper _encountersMapper;
        private readonly MicrotestMyRecordReferralsMapper _referralsMapper;

        public MicrotestMyRecordDetailedRecordMapper(
            MicrotestMyRecordImmunisationsMapper immunisationsMapper,
            MicrotestMyRecordProblemsMapper problemsMapper,
            MicrotestMyRecordTestResultsMapper testResultsMapper,
            MicrotestMyRecordMedicalHistoryMapper medicalHistoryMapper,
            MicrotestMyRecordRecallsMapper recallsMapper,
            MicrotestMyRecordEncountersMapper encountersMapper,
            MicrotestMyRecordReferralsMapper referralsMapper)
        {
            _immunisationsMapper = immunisationsMapper;
            _problemsMapper = problemsMapper;
            _testResultsMapper = testResultsMapper;
            _medicalHistoryMapper = medicalHistoryMapper;
            _recallsMapper = recallsMapper;
            _encountersMapper = encountersMapper;
            _referralsMapper = referralsMapper;
        }
        
        internal void Map(MyRecordResponse myRecordResponse, PatientRecordGetResponse patientRecordGetResponse)
        {
            _immunisationsMapper.Map(myRecordResponse, patientRecordGetResponse.ImmunisationData);
            _problemsMapper.Map(myRecordResponse, patientRecordGetResponse.ProblemData);
            _testResultsMapper.Map(myRecordResponse, patientRecordGetResponse.TestResultData);
            _medicalHistoryMapper.Map(myRecordResponse, patientRecordGetResponse.MedicalHistoryData);
            _recallsMapper.Map(myRecordResponse, patientRecordGetResponse.RecallData);
            _encountersMapper.Map(myRecordResponse, patientRecordGetResponse.EncounterData);
            _referralsMapper.Map(myRecordResponse, patientRecordGetResponse.ReferralData);
        }
    }
}