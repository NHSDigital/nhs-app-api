package mocking.vision.appointments.helpers

import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractReferencesFromFacade
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractSlotsFromFacade
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractVPBookingReasonsFromFacade
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper.Companion.extractVPCancelReasonsFromFacade
import mocking.vision.models.appointments.AppointmentSettings
import mocking.vision.models.appointments.BookedAppointmentsResponse
import mockingFacade.appointments.AppointmentSlotsResponseFacade

class MyAppointmentsHelper {

    companion object {

        fun extractResponseFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                BookedAppointmentsResponse {
            val cancellationReasons = extractVPCancelReasonsFromFacade(slotsResponseFacade)
            val bookingReason = extractVPBookingReasonsFromFacade(slotsResponseFacade)
            val settings = AppointmentSettings(cancellationReasons = cancellationReasons, bookingReason = bookingReason)
            return BookedAppointmentsResponse(
                    extractSlotsFromFacade(slotsResponseFacade),
                    extractReferencesFromFacade(slotsResponseFacade),
                    settings
            )
        }
    }
}
