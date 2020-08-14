package features.myrecord.factories

import mocking.MockingClient
import mocking.data.myrecord.DiagnosisData
import models.Patient
import mocking.vision.VisionConstants.diagnosisView
import mocking.vision.VisionConstants.htmlResponseFormat

class DiagnosisFactoryVision {

    private val mockingClient = MockingClient.instance

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                diagnosisView,
                htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }

    fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                diagnosisView,
                htmlResponseFormat) {
            request -> request.respondWithSuccess(DiagnosisData.getVisionDiagnosisDataWithMultipleResults())
        }
    }

    fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                diagnosisView,
                htmlResponseFormat) {
            request -> request.respondWithServiceUnavailable()
        }
    }

    fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                diagnosisView,
                htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }
}
