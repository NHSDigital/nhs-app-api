package mocking.emis.appointments.helpers

import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mockingFacade.appointments.AppointmentSessionFacade

class AppointmentSlotsMetaHelper {

    companion object {

        private const val NUMBER_OF_SLOTS: Int = 1
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
                    sessions.flatMap { session ->
                        session.slots.map { slot ->
                            Session(session.sessionType!!,
                                    slot.slotId!!,
                                    session.locationid,
                                    DEFAULT_DURATION,
                                    SessionType.Timed,
                                    NUMBER_OF_SLOTS,
                                    getArrayListOfSessionHoldersFromSession(session).map { it -> it.clinicianId },
                                    slot.startTime,
                                    slot.endTime)
                        }
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
