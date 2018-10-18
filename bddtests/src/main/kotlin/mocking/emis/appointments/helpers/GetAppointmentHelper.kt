package mocking.emis.appointments.helpers

import constants.DateTimeFormats
import mocking.emis.models.Appointment
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mockingFacade.appointments.MyAppointmentsFacade
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class GetAppointmentHelper {

    companion object {

        fun extractListOfAppointmentsFromFacade(facade: MyAppointmentsFacade): List<Appointment> {
            return facade.slots?.sessions?.flatMap { session ->
                session.slots.map { slot ->
                    Appointment(
                            slot.slotId!!,
                            session.sessionId!!,
                            convertDateToEmisTime(slot.startTime!!),
                            convertDateToEmisTime(slot.endTime!!),
                            slotTypeName = slot.slotTypeName!!
                    )
                }
            } ?: emptyList()
        }

        private fun convertDateToEmisTime(time: String): String {
            val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
            val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
            ("Europe/London"))
            val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            return queryDateFormat.format(dateToPass)
        }

        fun extractLocationsFromFacade(facade: MyAppointmentsFacade): List<Location> {
            return facade.slots?.sessions?.map { session -> Location(session.locationid!!, session.location!!) }
                    ?: emptyList()
        }

        fun extractCliniciansFromFacade(facade: MyAppointmentsFacade): List<SessionHolder> {
            val cliniciansAcrossAllSessions = facade.slots?.sessions?.flatMap { session ->
                session.staffDetails.map { clinician ->
                    SessionHolder(clinician.staffDetailsid!!, clinician.staffName!!)
                }
            } ?: emptyList()
            return cliniciansAcrossAllSessions.distinct()
        }

        fun extractSessionsFromFacade(facade: MyAppointmentsFacade): List<Session> {
            return facade.slots?.sessions?.map { session ->
                Session(
                        session.sessionType!!,
                        session.sessionId!!,
                        session.locationid,
                        clinicianIds = session.staffDetails.map { staff -> staff.staffDetailsid!! }
                )
            } ?: emptyList()

        }
    }
}