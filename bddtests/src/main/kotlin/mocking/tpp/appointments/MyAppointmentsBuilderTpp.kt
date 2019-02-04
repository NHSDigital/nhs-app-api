package mocking.tpp.appointments

import constants.DateTimeFormats
import mocking.JSonXmlConverter
import mocking.defaults.TppMockDefaults
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Appointment
import mocking.tpp.models.ViewAppointmentsReply
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
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
                                "@unitId='${TppMockDefaults.DEFAULT_ODS_CODE_TPP}' and " +
                                "@patientId='${patient.patientId}' and " +
                                "@onlineUserId='${patient.onlineUserId}']")
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return responseErrorWhenGPDisabledAppointmentsService()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithTppUnknownError("Unknown exception")
    }

    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val viewAppointmentsReply = viewAppointmentsReplyBase()
        if (facade.myAppointments != null) {
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
        return facade.myAppointments?.sessions?.flatMap { session ->
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

    override fun respondWithCorrupted(): Mapping {

        val response = JSonXmlConverter.toXML(viewAppointmentsReplyBase())
        return respondWithCorruptedContent(response)
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }
}
