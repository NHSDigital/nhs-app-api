package mocking.vision.appointments

import mocking.emis.practices.NecessityOption
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getVisionExistingAppointmentsResponse
import mocking.vision.VisionMappingBuilder
import mocking.vision.appointments.helpers.MyAppointmentsHelper.Companion.extractResponseFromFacade
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.BookedAppointmentsResponse
import mocking.vision.models.appointments.AppointmentSettings
import mocking.vision.models.appointments.BookingReason
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller
import utils.SerenityHelpers.Companion.getValueOrNull


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

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondVisionErrorWhenServiceNotEnabled(serviceDefinition)
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithUnknownVisionError(serviceDefinition)
    }
    private fun createDefaultResponse() :BookedAppointmentsResponse {
        return BookedAppointmentsResponse(
                settings = AppointmentSettings(
                        bookingReason = BookingReason(
                                add = bookingReasonNecessity()
                        )
                )
        )
    }
    private fun bookingReasonNecessity() : Boolean{
        return getValueOrNull<NecessityOption>("BookingReasonNecessity")  != NecessityOption.NOT_ALLOWED
    }
    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val bookedAppointmentsResponse =
                if (facade.slots != null)
                    extractResponseFromFacade(facade.slots)
                else
                    createDefaultResponse()


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

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent(serviceDefinition)
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }

}
