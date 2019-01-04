package mockingFacade.appointments

import mocking.emis.models.AppointmentCancellationReason
import mocking.emis.models.SlotTypeStatus
import mocking.emis.practices.NecessityOption


data class AppointmentSlotsResponseFacade(
        val sessions: ArrayList<AppointmentSessionFacade> = arrayListOf(),
        val bookableDays: String? = null,
        var cancellationReasons: List<AppointmentCancellationReason>? = null,
        var bookingReasonNecessityOption: NecessityOption = NecessityOption.OPTIONAL,
        var appointmentSlotTypeStatus: SlotTypeStatus = SlotTypeStatus.Unknown
)