package mocking.vision.appointments.helpers

import mocking.vision.models.appointments.BookedAppointmentsResponse
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import mocking.vision.models.appointments.Slot
import mocking.vision.models.appointments.SlotType
import mocking.vision.models.appointments.Slots
import mockingFacade.appointments.AppointmentSlotsResponseFacade

class MyAppointmentsHelper {

    companion object {

        fun extractResponseFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                BookedAppointmentsResponse {
            return BookedAppointmentsResponse(
                    extractSlotsFromFacade(slotsResponseFacade),
                    extractReferencesFromFacade(slotsResponseFacade)
            )
        }

        private fun extractSlotsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): Slots {
            return Slots(
                    slotsResponseFacade.sessions.flatMap { session ->
                        session.slots.map { slot ->
                            Slot(
                                    slot.slotId!!.toString(),
                                    slot.startTime!!,
                                    session.staffDetails.first().staffDetailsid.toString(),
                                    session.locationid,
                                    session.sessionType,
                                    session.sessionId
                            )
                        }
                    }
            )
        }

        private fun extractReferencesFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): References {
            return References(
                    extractLocationsFromFacade(slotsResponseFacade),
                    extractOwnersFromFacade(slotsResponseFacade),
                    extractSessionsFromFacade(slotsResponseFacade),
                    extractSlotTypesFromFacade(slotsResponseFacade)
            )
        }

        private fun extractLocationsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                List<mocking.vision.models.appointments.Location> {
            return slotsResponseFacade.sessions.map { session ->
                mocking.vision.models.appointments.Location(session.locationid, session.location!!)
            }.distinct()
        }

        private fun extractOwnersFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<Owner> {
            return slotsResponseFacade.sessions.flatMap { session ->
                session.staffDetails.map { clinician ->
                    Owner(clinician.staffDetailsid!!, clinician.staffName!!)
                }
            }.distinct()
        }

        private fun extractSessionsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                List<mocking.vision.models.appointments.Session> {
            return slotsResponseFacade.sessions.map { session ->
                mocking.vision.models.appointments.Session(session.sessionId, session.sessionDetails!!)
            }
        }

        private fun extractSlotTypesFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<SlotType> {
            return slotsResponseFacade.sessions.flatMap { session ->
                session.slots.map { slot ->
                    SlotType(slot.slotTypeId!!, slot.slotTypeName!!)
                }
            }.distinct()
        }
    }
}
