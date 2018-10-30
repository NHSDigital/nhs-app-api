package mocking.vision.appointments.helpers

import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.calculateDuration
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.convertDateToVisionTime
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractReferencesFromFacade
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractSlotsFromFacade
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
    }
}
