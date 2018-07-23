package mockingFacade.appointments

data class AppointmentSessionFacade(
        var sessionDate: String? = null,
        var sessionId: Int? = null,
        var sessionType: String? = null,
        var staffDetails: String? = null,
        var location: String? = null,
        var slots: ArrayList<AppointmentSlotFacade> = ArrayList()
)