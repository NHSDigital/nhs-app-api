package worker.models.appointments

data class AppointmentResponseObject(
        val id: String,
        val type: String,
        val sessionName: String?,
        val startTime: String,
        val endTime: String? = null,
        val location: String,
        val clinicians: List<String> = emptyList(),
        val disableCancellation: String? = "false"
)