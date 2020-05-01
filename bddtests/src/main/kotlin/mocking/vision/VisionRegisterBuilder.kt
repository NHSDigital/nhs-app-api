package mocking.vision

import mocking.JSonXmlConverter
import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionResponse
import mocking.vision.VisionErrorResponses.getConnectionToExternalServiceFailedError
import mocking.vision.VisionErrorResponses.getInvalidDetailsProvidedError
import mocking.vision.VisionErrorResponses.getInvalidParameterProvidedError
import mocking.vision.VisionErrorResponses.getPatientAlreadyRegisteredError
import mocking.vision.VisionErrorResponses.getPatientLockedError
import mocking.vision.VisionErrorResponses.getMockedError
import mocking.vision.models.Register
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import models.Patient
import org.apache.http.HttpStatus

class VisionRegisterBuilder(var userSession: VisionUserSession,
                            var serviceDefinition: ServiceDefinition, patient: Patient) : VisionMappingBuilder("POST") {

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
                .andBody(patient.name.surname, "contains")
                .andBody(patient.age.dateOfBirth, "contains")
                .andBody(patient.linkageKey, "contains")
    }

    fun respondWithSuccess(register: Register): Mapping {

        val xmlBody = JSonXmlConverter.toXML(register)

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionResponse(xmlBody, serviceDefinition)).build()
        }
        return resp
    }

    fun respondWithError(typeOfError: String): Mapping {

        return respondWith(HttpStatus.SC_OK) {
            when (typeOfError) {

                "Invalid Details" -> andXmlBody(getInvalidDetailsProvidedError(serviceDefinition)).build()
                "Invalid Parameter" -> andXmlBody(getInvalidParameterProvidedError(serviceDefinition)).build()
                "Already Registered" -> andXmlBody(getPatientAlreadyRegisteredError(serviceDefinition)).build()
                "Patient Locked" -> andXmlBody(getPatientLockedError(serviceDefinition)).build()
                "Vision Connection To External Service Failed" ->
                    andXmlBody(getConnectionToExternalServiceFailedError(serviceDefinition)).build()
                else -> throw IllegalArgumentException("$typeOfError is not recognised as an Error Type")
            }
        }
    }

    fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        return respondWith(httpStatusCode) {
            andXmlBody(
                    getMockedError(serviceDefinition, errorCode, message ?: "Mocked Error")).build()
        }
    }
}

