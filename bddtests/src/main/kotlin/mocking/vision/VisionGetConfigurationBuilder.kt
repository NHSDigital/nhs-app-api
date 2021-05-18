package mocking.vision

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionDirectServicesResponse
import mocking.vision.VisionDirectServicesErrorResponses.getInvalidRequestError
import mocking.vision.VisionDirectServicesErrorResponses.getInvalidUserCredentialsError
import mocking.vision.VisionDirectServicesErrorResponses.getUnknownError
import mocking.vision.VisionDirectServicesErrorResponses.getConnectionToExternalServiceFailedError
import mocking.vision.VisionDirectServicesErrorResponses.getMockedError
import mocking.vision.VisionDirectServicesErrorResponses.getPatientLockedError
import mocking.vision.VisionDirectServicesErrorResponses.getRegistrationIncomplete
import mocking.vision.models.Configuration
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.WelcomeText
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionGetConfigurationBuilder(var userSession: VisionUserSession,
                                    var serviceDefinition: ServiceDefinition) :
    VisionDirectServicesMappingBuilder(orgId = userSession.odsCode, path = "configuration") {

    init {
        requestBuilder
            .andBody("<vision:rosuAccountId>${userSession.rosuAccountId}</vision:rosuAccountId>", "contains")
            .andBody("<vision:apiKey>${userSession.apiKey}</vision:apiKey>", "contains")
            .andBody("<vision:provider>${userSession.provider}</vision:provider>", "contains")
            .andBody("<vision:accountId>${userSession.accountId}</vision:accountId>", "contains")
    }

    fun respondWithSuccess(configuration: Configuration): Mapping {
        if (Serenity.hasASessionVariableCalled(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)) {
            configuration.appointments.welcomeText = WelcomeText(
                    message = "<HTML><BODY>" +
                            Serenity.sessionVariableCalled<String>(
                                    AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY) + "</BODY></HTML>")
        }
        val jaxbContext = JAXBContext.newInstance(Configuration::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(configuration, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionDirectServicesResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }

    fun respondWithInvalidRequest(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getInvalidRequestError(serviceDefinition)).build()
        }
    }

    fun respondWithUnknownError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getUnknownError(serviceDefinition)).build()
        }
    }

    fun respondWithRecordCurrentlyUnavailableError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getPatientLockedError(serviceDefinition)).build()
        }
    }

    fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        return respondWith(httpStatusCode) {
            andXmlBody(getMockedError(serviceDefinition, errorCode, message ?: "Mocked Error")).build()
        }
    }

    fun respondWithInvalidUserCredentials(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getInvalidUserCredentialsError(serviceDefinition)).build()
        }
    }

    fun respondWithConnectionToExternalServiceFailed(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getConnectionToExternalServiceFailedError(serviceDefinition)).build()
        }
    }

    fun respondWithRegistrationIncomplete(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getRegistrationIncomplete(serviceDefinition)).build()
        }
    }
}
