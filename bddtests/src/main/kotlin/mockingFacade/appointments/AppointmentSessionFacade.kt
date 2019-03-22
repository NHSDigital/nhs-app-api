package mockingFacade.appointments

data class AppointmentSessionFacade(
        var sessionDate: String? = null,
        var sessionId: Int? = null,
        var sessionType: String? = null,
        var staffDetails: List<Int> = arrayListOf(),
        var locationId: Int? = null,
        var slots: List<AppointmentSlotFacade> = arrayListOf()
)