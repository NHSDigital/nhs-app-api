package mockingFacade.appointments

import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.practices.NecessityOption
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade


data class AppointmentSlotsResponseFacade(
        var sessions: List<AppointmentSessionFacade> = listOf(),
        var bookableDays: String = "1",
        var locations: List<LocationFacade> = listOf(),
        var staffDetails: List<StaffDetailsFacade> = listOf(),
        var slotTypes: List<SlotTypeFacade> = listOf(),
        var cancellationReasons: List<AppointmentCancellationReason>? = null,
        var bookingReasonNecessityOption: NecessityOption = NecessityOption.OPTIONAL
)
