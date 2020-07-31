package mocking.tpp.appointments

import constants.DateTimeFormats
import mocking.JSonXmlConverter
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Appointment
import mocking.tpp.models.ViewAppointmentsReply
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper
import models.Patient
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter


class MyAppointmentsBuilderTpp(
        val patient: Patient,
        appointmentType: IMyAppointmentsBuilder.AppointmentType
) : TppMappingBuilder(), IMyAppointmentsBuilder {

    private val tppAppointmentType: AppointmentType

    init {
        tppAppointmentType = when (appointmentType) {
            IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY -> AppointmentType.FUTURE_APPOINTMENTS
            IMyAppointmentsBuilder.AppointmentType.PAST_ONLY -> AppointmentType.PAST_APPOINTMENTS
            else -> throw NullPointerException("Appointment Type incorrectly set for TPP. Need to specify " +
                    "either Upcoming or Past (not both). ")
        }
        requestBuilder.andHeader(HEADER_TYPE, "ViewAppointments")
                .andBodyMatchingXpath(
                        xpath = "//ViewAppointments[" +
                                "@apiVersion='${TppConfig.apiVersion}' and " +
                                "@unitId='${patient.tppUserSession!!.unitId}' and " +
                                "@patientId='${patient.patientId}' and " +
                                "@onlineUserId='${patient.onlineUserId}' and " +
                                "@futureAppointments='${tppAppointmentType.isFuture}']")
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return responseErrorWhenGPDisabledAppointmentsService()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithTppUnknownError("Unknown exception")
    }

    override fun respondWithUnauthorised(): Mapping {
        return respondWithTppNotAuthorised("Not authorized")
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
        val requiredAppointments = when (tppAppointmentType) {
            AppointmentType.FUTURE_APPOINTMENTS ->
                MyAppointmentFacadeHelper.upcomingAppointmentsFromSessions(facade.myAppointments!!.sessions)
            AppointmentType.PAST_APPOINTMENTS ->
                MyAppointmentFacadeHelper.historicalAppointmentsFromSessions(facade.myAppointments!!.sessions)
        }
        return requiredAppointments.flatMap { session ->
            session.slots.map { slot ->
                Appointment(
                        slot.slotId!!.toString(),
                        convertDateToTppTime(slot.startTime!!),
                        convertDateToTppTime(slot.endTime!!),
                        slot.slotDetails,
                        facade.myAppointments.locations.find { location -> location.locationId == session.locationId
                        }!!.locationName
                )
            }
        }
    }

    private fun convertDateToTppTime(time: String): String {
        val timeZone = "[Europe/London]"
        val timeZoneWithOffset = "+01:00[Europe/London]"
        if(time.contains(timeZone)) {
            return time
                    .replace(timeZoneWithOffset, "Z")
                    .replace(timeZone,"")
        }
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

    enum class AppointmentType(val isFuture: Char) {
        FUTURE_APPOINTMENTS('Y'),
        PAST_APPOINTMENTS('N')
    }
}
