package mockingFacade.appointments

data class AppointmentSessionFacade(
        var sessionDate: String? = null,
        var sessionId: Int? = null,
        var sessionType: String? = null,
        var staffDetails: List<StaffDetailsFacade> = arrayListOf(),
        var location: String? = null,
        var slots: List<AppointmentSlotFacade> = arrayListOf(),
        var locationid: Int? = null,
        var sessionDetails: String? = null
)