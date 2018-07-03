package worker.models.demographics

data class TppUserSession(
        var suid: String,
        var patientId: String,
        var unitId: String,
        var onlineUserId: String
)
