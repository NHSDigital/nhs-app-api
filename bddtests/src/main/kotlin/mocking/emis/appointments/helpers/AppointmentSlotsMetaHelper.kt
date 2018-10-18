package mocking.emis.appointments.helpers

import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mockingFacade.appointments.AppointmentSessionFacade

class AppointmentSlotsMetaHelper {

    companion object {
        
        private const val DEFAULT_DURATION: Int = 10

        fun getMetaSlotLocationsList(sessions: ArrayList<AppointmentSessionFacade>):
                ArrayList<Location> {
            return ArrayList(sessions.map { session -> Location(session.locationid!!, session.location!!) })
        }

        fun getMetaSlotSessionHoldersList(sessions: ArrayList<AppointmentSessionFacade>):
                ArrayList<SessionHolder> {
            var set = setOf<SessionHolder>()
            for (session in sessions) {
                val arrayList = getArrayListOfSessionHoldersFromSession(session)
                set = set.union(arrayList)
            }
            return ArrayList(set)
        }

        fun getMetaSlotSessionsList(sessions: ArrayList<AppointmentSessionFacade>):
                ArrayList<Session> {
            return ArrayList(
                    sessions.map { session ->
                        Session(
                                session.sessionType!!,
                                session.sessionId!!,
                                session.locationid,
                                DEFAULT_DURATION,
                                SessionType.Timed,
                                session.slots.size,
                                getArrayListOfSessionHoldersFromSession(session).map { it -> it.clinicianId }
                        )
                    }
            )
        }

        private fun getArrayListOfSessionHoldersFromSession(session: AppointmentSessionFacade):
                ArrayList<SessionHolder> {
            return ArrayList(
                    session.staffDetails.map { clinician ->
                        SessionHolder(clinician.staffDetailsid!!, clinician.staffName)
                    }
            )
        }
    }
}
