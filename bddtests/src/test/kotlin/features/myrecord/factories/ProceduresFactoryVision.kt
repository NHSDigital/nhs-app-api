package features.myrecord.factories

import mocking.MockingClient
import mocking.data.myrecord.ProceduresData
import mocking.vision.VisionConstants
import models.Patient

class ProceduresFactoryVision {

    private val mockingClient = MockingClient.instance

    private var mocker: MyRecordVisionMocker = MyRecordVisionMocker(mockingClient)

    fun disabled(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }

    fun enabledWithRecords(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithSuccess(ProceduresData.getVisionProceduresDataWithMultipleProcedures())
        }
    }

    fun errorRetrieving(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithServiceUnavailable()
        }
    }

    fun noAccess(patient: Patient) {
        mocker.generatePatientDataResponse(
                patient,
                VisionConstants.proceduresView,
                VisionConstants.htmlResponseFormat) {
            request -> request.respondWithAccessDeniedError()
        }
    }
}
