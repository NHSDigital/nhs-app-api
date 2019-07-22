package features.myrecord.factories

import mocking.data.myrecord.ProblemsData
import mocking.vision.VisionConstants
import models.Patient
import worker.models.myrecord.ProblemItem

class ProblemsFactoryVision : ProblemsFactory() {

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)


    override fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithAccessDeniedError() }
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithSuccess(ProblemsData.getVisionProblemsDataWithNoProblems()) }
    }

    override fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithSuccess(ProblemsData.getVisionProblemsData()) }
    }
    override fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithUnknownError() }
    }

    override fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(patient, VisionConstants.problemsView)
        { request -> request.respondWithAccessDeniedError() }
    }

    override fun getExpectedProblems(): List<ProblemItem> {
        throw UnsupportedOperationException()
    }
}