package mocking.vision.appointments

import constants.DateTimeFormats
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionMappingBuilder
import mocking.vision.appointments.helpers.AppointmentsSlotsHelper
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.AvailableAppointmentsResponse
import mocking.vision.models.appointments.BookedAppointmentsResponse
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import org.apache.http.HttpStatus
import worker.models.appointments.AppointmentSlotsResponse
import java.io.StringWriter
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class AppointmentSlotsBuilderVision(patient: Patient, fromDateString: String?, toDateString: String?)
    : VisionMappingBuilder(), IAppointmentSlotsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.availableAppointmentsName,
            VisionConstants.availableAppointmentsVersion
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

        if (fromDateString != null) {
            requestBuilder.andBody(convertDateToVisionTime(fromDateString), "contains")
        }

        if (toDateString != null) {
            requestBuilder.andBody(convertDateToVisionTime(toDateString), "contains")
        }
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        val availableAppointmentsResponse =
                if (!facade.sessions.isEmpty())
                    AppointmentsSlotsHelper.extractResponseFromFacade(facade)
                else
                    AvailableAppointmentsResponse()

        val jaxbContext = JAXBContext.newInstance(AvailableAppointmentsResponse::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(availableAppointmentsResponse, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(
                    VisionConstants.getVisionAvailableAppointmentsResponse(stringWriter.toString(), serviceDefinition)
            ).andDelay(delayMillisecs).build()
        }
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenNotEnabled() is not yet implemented in " +
                        "AppointmentsSlotsVision")
    }

    override fun respondWithUnknownException(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithUnknownException() is not yet implemented in " +
                        "AppointmentsSlotsVision")
    }

    private fun convertDateToVisionTime(time: String): String {
        val dateToPass = ZonedDateTime.parse(time)
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(dateToPass)
    }
}