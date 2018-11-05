package mocking.vision.appointments

import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getVisionExistingAppointmentsResponse
import mocking.vision.VisionMappingBuilder
import mocking.vision.appointments.helpers.MyAppointmentsHelper.Companion.extractResponseFromFacade
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.BookedAppointmentsResponse
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller


class MyAppointmentsBuilderVision(val patient: Patient) : VisionMappingBuilder(), IMyAppointmentsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.existingAppointmentsName,
            VisionConstants.existingAppointmentsVersion
    )

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val userSession = VisionUserSession(
                patient.rosuAccountId,
                patient.apiKey,
                patient.odsCode,
                patient.patientId
        )

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
                .andBody(userSession.patientId, "contains")
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenNotEnabled() is not yet implemented in " +
                        "MyAppointmentsBuilderVision")
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("").build()
        }
    }

    override fun respondWithUnknownException(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithUnknownException() is not yet implemented in " +
                        "MyAppointmentsBuilderVision")
    }

    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val bookedAppointmentsResponse =
                if (facade.slots != null)
                    extractResponseFromFacade(facade.slots)
                else
                    BookedAppointmentsResponse()


        val jaxbContext = JAXBContext.newInstance(BookedAppointmentsResponse::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(bookedAppointmentsResponse, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionExistingAppointmentsResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWithBody(body)
    }

    override fun respondWithCorrupted(facade: MyAppointmentsFacade): Mapping {
        val mapping = respondWithSuccess(facade)
        return respondWith(HttpStatus.SC_OK) {
            andBody(mapping.response!!.body!!.replace(">", "|").replace("}", "|"), contentType = "application/xml")
        }

    }

}
