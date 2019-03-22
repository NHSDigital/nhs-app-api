package mocking.emis.appointments.helpers

import constants.DateTimeFormats
import mocking.emis.models.Appointment
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class GetAppointmentHelper {

    companion object {

        private const val DEFAULT_DURATION: Int = 10

        fun extractListOfAppointmentsFromFacade(facade: AppointmentSlotsResponseFacade): List<Appointment> {
            return facade.sessions.flatMap { session ->
                session.slots.map { slot ->
                    Appointment(
                            slot.slotId!!,
                            session.sessionId!!,
                            convertDateToEmisTime(slot.startTime!!),
                            convertDateToEmisTime(slot.endTime!!),
                            slotTypeName = facade.slotTypes.find { slotType ->
                                slotType.slotTypeId == slot.slotTypeId
                            }!!.slotTypeName
                    )
                }
            }
        }

        private fun convertDateToEmisTime(time: String): String {
            val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
            val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
            ("Europe/London"))
            val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            return queryDateFormat.format(dateToPass)
        }

        fun extractLocationsFromFacade(facade: AppointmentSlotsResponseFacade): List<Location> {
            return facade.locations.map { location ->
                Location(location.locationId, location.locationName)
            }
        }

        fun extractCliniciansFromFacade(facade: AppointmentSlotsResponseFacade): List<SessionHolder> {
            return facade.staffDetails.map { staffDetails ->
                SessionHolder(staffDetails.staffDetailsid, staffDetails.staffName)
            }
        }

        fun extractSessionsFromFacade(facade: AppointmentSlotsResponseFacade): List<Session> {
            return facade.sessions.map { session ->
                Session(
                        session.sessionType!!,
                        session.sessionId!!,
                        session.locationId,
                        DEFAULT_DURATION,
                        SessionType.Timed,
                        session.slots.size,
                        session.staffDetails
                )
            }
        }
    }
}
