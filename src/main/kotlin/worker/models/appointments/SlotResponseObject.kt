package worker.models.appointments


data class SlotResponseObject(
        var id: String,
        var startTime: String,
        var endTime: String,
        var locationId: String,
        var appointmentSessionId: String,
        var clinicianIds: Array<String>
)