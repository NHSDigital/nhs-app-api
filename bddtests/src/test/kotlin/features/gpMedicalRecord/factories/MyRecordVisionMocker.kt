package features.gpMedicalRecord.factories

import mocking.MockingClient
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionGetPatientDataBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient

class MyRecordVisionMocker(val mockingClient: MockingClient){

    fun generatePatientDataResponse(
            patient: Patient,
            view: String,
            result: (VisionGetPatientDataBuilder) -> Mapping) {
        generatePatientDataResponse(patient, view, VisionConstants.xmlResponseFormat, result)
    }

    fun generatePatientDataResponse(
            patient: Patient,
            view: String,
            responseFormat: String,
            result: (VisionGetPatientDataBuilder) -> Mapping) {
        mockingClient.forVision {
            result(myRecord.getPatientDataRequest(
                    visionUserSession = VisionUserSession.fromPatient(patient),
                    serviceDefinition = serviceDefinition,
                    view = view,
                    responseFormat = responseFormat
            ))
        }
    }

    private var serviceDefinition = ServiceDefinition(
            name = VisionConstants.patientDataName,
            version = VisionConstants.patientDataVersion)
}