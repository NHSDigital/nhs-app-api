package mocking.vision.appointments.helpers

import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractReferencesFromFacade
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractSlotsFromFacade
import mocking.vision.models.appointments.AvailableAppointmentsResponse
import mockingFacade.appointments.AppointmentSlotsResponseFacade

class AppointmentsSlotsHelper {
    companion object {

        fun extractResponseFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                AvailableAppointmentsResponse {
            return AvailableAppointmentsResponse(
                    extractSlotsFromFacade(slotsResponseFacade),
                    extractReferencesFromFacade(slotsResponseFacade)
            )
        }
    }
}
