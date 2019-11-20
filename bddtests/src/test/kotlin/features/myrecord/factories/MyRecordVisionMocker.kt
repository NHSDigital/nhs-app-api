package features.myrecord.factories

import mocking.MockingClient
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionGetPatientDataBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient
import org.junit.Assert

class MyRecordVisionMocker(val mockingClient: MockingClient) {

    fun generatePatientDataResponse(
            patient: Patient,
            view: String,
            result: (VisionGetPatientDataBuilder) -> Mapping) {
        val responseFormat = getResponseFormatting(view)
        generatePatientDataResponse(patient, view, responseFormat, result)
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


    private fun getResponseFormatting(view: String): String {
        if (view == VisionConstants.proceduresView) {
            Assert.fail("PROCEDURES response format must be set explicitly, " +
                    "as both immunisations and procedures use it")
        }
        val responseFormatMapping = mapOf(
                VisionConstants.allergiesView to VisionConstants.xmlResponseFormat,
                VisionConstants.medicationsView to VisionConstants.xmlResponseFormat,
                VisionConstants.testResultsView to VisionConstants.htmlResponseFormat,
                VisionConstants.problemsView to VisionConstants.xmlResponseFormat,
                VisionConstants.diagnosisView to VisionConstants.htmlResponseFormat,
                VisionConstants.examinationsView to VisionConstants.htmlResponseFormat)
        if (!responseFormatMapping.containsKey(view)) {
            Assert.fail("Unknown response format for $view")
        }
        return responseFormatMapping.getValue(view)
    }
}