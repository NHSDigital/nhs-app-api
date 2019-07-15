package worker.models.appointments

import mocking.emis.models.SlotTypeStatus

data class AppointmentResponseObject(
        val id: String,
        val type: String,
        val sessionName: String?,
        val startTime: String,
        val endTime: String? = null,
        val location: String,
        val clinicians: List<String> = emptyList(),
        var disableCancellation: String? = "false",
        var channel: SlotTypeStatus? = SlotTypeStatus.Unknown,
        val telephoneNumber: String = ""
)