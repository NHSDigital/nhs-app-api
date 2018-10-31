package mockingFacade.appointments

import mocking.emis.models.AppointmentCancellationReason


data class AppointmentSlotsResponseFacade(
        val sessions: ArrayList<AppointmentSessionFacade> = arrayListOf(),
        val bookableDays: String? = null,
        var cancellationReasons: List<AppointmentCancellationReason>? = null
)