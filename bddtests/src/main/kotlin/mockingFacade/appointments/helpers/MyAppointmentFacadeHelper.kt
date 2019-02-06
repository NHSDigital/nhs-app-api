package mockingFacade.appointments.helpers

import mockingFacade.appointments.AppointmentSessionFacade
import java.time.ZonedDateTime

class MyAppointmentFacadeHelper {

    companion object {

        fun historicalAppointmentsFromSessions(
                sessions: List<AppointmentSessionFacade>
        ): List<AppointmentSessionFacade> {
            return sessions.filter { session ->
                session.slots.any { slot ->
                    ZonedDateTime.parse(slot.startTime).isBefore(ZonedDateTime.now())
                }
            }
        }

        fun upcomingAppointmentsFromSessions(
                sessions: List<AppointmentSessionFacade>
        ): List<AppointmentSessionFacade> {
            return sessions.filter { session ->
                session.slots.any { slot ->
                    ZonedDateTime.parse(slot.startTime).isAfter(ZonedDateTime.now())
                }
            }
        }

    }
}
