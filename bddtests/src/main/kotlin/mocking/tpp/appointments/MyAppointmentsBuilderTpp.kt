package mocking.tpp.appointments

import constants.DateTimeFormats
import mocking.JSonXmlConverter
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Appointment
import mocking.tpp.models.ViewAppointmentsReply
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter


class MyAppointmentsBuilderTpp(val patient: Patient) : TppMappingBuilder(), IMyAppointmentsBuilder {

    init {
        requestBuilder.andHeader(HEADER_TYPE, "ViewAppointments")
                .andBodyMatchingXpath(
                        xpath = "//ViewAppointments[" +
                                "@apiVersion='${TppConfig.apiVersion}' and " +
                                "@unitId='${TppConfig.unitId}' and " +
                                "@patientId='${patient.patientId}' and " +
                                "@onlineUserId='${patient.onlineUserId}']")
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorWhenGPDisabledAppointmentsService()
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("Service unavailable").build()
        }
    }

    override fun respondWithUnknownException(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithUnknownException() is not yet implemented in " +
                        "MyAppointmentsBuilderTpp")
    }

    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val viewAppointmentsReply = viewAppointmentsReplyBase()
        if (facade.slots != null) {
            viewAppointmentsReply.Appointment = extractAppointmentsFromFacade(facade)
        }
        return respondWithSuccess(JSonXmlConverter.toXML(viewAppointmentsReply))
    }

    private fun viewAppointmentsReplyBase(): ViewAppointmentsReply {
        return ViewAppointmentsReply(
                patientId = patient.patientId,
                onlineUserId = patient.onlineUserId,
                uuid = TppConfig.uuid)
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWithBody(body)
    }

    private fun extractAppointmentsFromFacade(facade: MyAppointmentsFacade): List<Appointment> {
        return facade.slots?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                Appointment(
                        slot.slotId!!.toString(),
                        convertDateToTppTime(slot.startTime!!),
                        convertDateToTppTime(slot.endTime!!),
                        session.sessionDetails!!,
                        session.location!!
                )
            }
        } ?: emptyList()
    }

    private fun convertDateToTppTime(time: String): String {
        val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
        ("Europe/London"))
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.tppDateTimeFormat)
        return queryDateFormat.format(dateToPass)
    }

    override fun respondWithCorrupted(facade: MyAppointmentsFacade): Mapping {
        val mapping = respondWithSuccess(facade)

        return respondWith(HttpStatus.SC_OK) {
            andBody(mapping.response!!.body!!.replace(">","|").replace("}","|"), contentType = "application/json")
        }
    }
}
