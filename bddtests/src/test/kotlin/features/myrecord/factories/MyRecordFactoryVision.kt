package features.myrecord.factories

import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.DiagnosisData
import mocking.data.myrecord.ExaminationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.ProblemsData
import mocking.data.myrecord.ProceduresData
import mocking.data.myrecord.TestResultsData
import mocking.vision.VisionConstants
import models.Patient
import mocking.data.myrecord.NUMBER_OF_ALLERGY_RECORDS
import mocking.data.myrecord.NUMBER_OF_IMMUNISATION_RECORDS
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions

class MyRecordFactoryVision: MyRecordFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)
    private val medicationsFactoryVision by lazy { MedicationsFactoryVision()}

    override fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView, VisionConstants.htmlResponseFormat)
        { request -> request.respondWithAccessDeniedError()}
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesData(0))}

        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView,
                VisionConstants.xmlResponseFormat)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())}

        mocker.generatePatientDataResponse(patient, VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())}

        mocker.generatePatientDataResponse(patient, VisionConstants.medicationsView)
         { request -> request.respondWithSuccess(ImmunisationsData.getVisionImmunisationsDataWithNoImmunisations())}

        mocker.generatePatientDataResponse(patient, VisionConstants.testResultsView)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults()) }

        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults()) }

        mocker.generatePatientDataResponse(patient, VisionConstants.diagnosisView)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults()) }

        mocker.generatePatientDataResponse(patient, VisionConstants.examinationsView)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults()) }
    }

    override fun enabledWithAllRecords(patient: Patient){
        mocker.generatePatientDataResponse(patient, VisionConstants.allergiesView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(AllergiesData.getVisionAllergiesData(NUMBER_OF_ALLERGY_RECORDS))}

        mocker.generatePatientDataResponse(patient, VisionConstants.immunisationsView,
                VisionConstants.xmlResponseFormat)
        { request -> request.respondWithSuccess(
                ImmunisationsData.getVisionImmunisationsData(NUMBER_OF_IMMUNISATION_RECORDS))}

        medicationsFactoryVision.enabledWithRecords(patient)

        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithSuccess(ProblemsData.getVisionProblemsData())}

        mocker.generatePatientDataResponse(patient, VisionConstants.testResultsView,
                VisionConstants.htmlResponseFormat)
        { request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithMultipleResults()) }

        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithSuccess(ProceduresData.getVisionProceduresDataWithMultipleProcedures())
        }

        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.examinationsView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithSuccess(ExaminationsData.getVisionExaminationsDataWithMultipleResults())
        }

        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.diagnosisView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithSuccess(DiagnosisData.getVisionDiagnosisDataWithMultipleResults())
        }
    }

    override fun enabledWithData(
            patient: Patient, myRecordModuleCounts: MyRecordModuleCounts, testResultOptions: TestResultOptions) {
        throw UnsupportedOperationException()
    }

    override fun respondWithForbidden(patient: Patient) {
        throw UnsupportedOperationException()
    }
}