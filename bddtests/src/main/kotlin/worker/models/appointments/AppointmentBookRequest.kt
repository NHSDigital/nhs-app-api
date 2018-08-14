package worker.models.appointments

data class AppointmentBookRequest(
        var userPatientLinkToken: String? = null,
        var slotId: String? = null,
        var bookingReason: String? = null,
        var startTime: String? =null,
        var endTime: String? = null
)