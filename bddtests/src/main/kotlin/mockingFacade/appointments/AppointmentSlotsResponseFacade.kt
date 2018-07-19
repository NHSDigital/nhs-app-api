package mockingFacade.appointments


data class AppointmentSlotsResponseFacade(
        val sessions: ArrayList<AppointmentSessionFacade> = arrayListOf(),
        val bookableDays : String? = null)