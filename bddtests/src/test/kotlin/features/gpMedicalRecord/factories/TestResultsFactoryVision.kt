package features.gpMedicalRecord.factories

import mocking.data.myrecord.TestResultsData
import mocking.vision.VisionConstants.htmlResponseFormat
import mocking.vision.VisionConstants.testResultsView
import models.Patient

class TestResultsFactoryVision : TestResultsFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    private val testResultsData by lazy { TestResultsData() }
    override fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                testResultsView,
                htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                testResultsView,
                htmlResponseFormat) {
            request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithNoTestResults())
        }
    }

    override fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                testResultsView,
                htmlResponseFormat) {
            request -> request.respondWithSuccess(TestResultsData.getVisionTestResultsDataWithMultipleResults())
        }
    }

    override fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                testResultsView,
                htmlResponseFormat) {
            request -> request.respondWithServiceUnavailable()
        }
    }

    override fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                testResultsView,
                htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }

}