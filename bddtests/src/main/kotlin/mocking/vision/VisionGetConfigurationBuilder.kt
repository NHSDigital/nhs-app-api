package mocking.vision

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionResponse
import mocking.vision.VisionErrorResponses.getInvalidRequestError
import mocking.vision.VisionErrorResponses.getInvalidUserCredentialsError
import mocking.vision.VisionErrorResponses.getUnknownError
import mocking.vision.VisionErrorResponses.securityHeaderErrorResponse
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
                                    var serviceDefinition: ServiceDefinition) : VisionMappingBuilder("POST") {

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
    }

    fun respondWithSuccess(configuration: Configuration): Mapping {
        if (Serenity.hasASessionVariableCalled(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)) {
            configuration.appointments.welcomeText = WelcomeText(
                    message = "<![CDATA[<HTML><BODY>" +
                            Serenity.sessionVariableCalled<String>(
                                    AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY) + "</BODY></HTML>]]>")
        }
        val jaxbContext = JAXBContext.newInstance(Configuration::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(configuration, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }

    fun respondWithInvalidRequest(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getInvalidRequestError(serviceDefinition)).build()
        }
    }

    fun respondWithSecurityHeaderError(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(securityHeaderErrorResponse).build()
        }
    }

    fun respondWithUnknownError(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getUnknownError(serviceDefinition)).build()
        }
    }

    fun respondWitInvalidUserCredentials(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getInvalidUserCredentialsError(serviceDefinition)).build()
        }
    }
}



