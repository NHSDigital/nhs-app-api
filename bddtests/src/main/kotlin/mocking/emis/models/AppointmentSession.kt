package mocking.emis.models

data class AppointmentSession(
        var sessionDate: String? = null,
        var sessionId: Int? = null,
        var slots: List<AppointmentSlot> = ArrayList()
)