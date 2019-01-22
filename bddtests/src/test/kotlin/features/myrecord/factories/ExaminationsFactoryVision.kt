package features.myrecord.factories

import mocking.MockingClient
import mocking.data.myrecord.ExaminationsData
import mocking.vision.VisionConstants
import models.Patient

class ExaminationsFactoryVision {

    val mockingClient = MockingClient.instance

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.examinationsView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }

    fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.examinationsView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithSuccess(ExaminationsData.getVisionTestResultsDataWithMultipleResults())
        }
    }

    fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.examinationsView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithServiceUnavailable()
        }
    }

    fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.examinationsView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }
}