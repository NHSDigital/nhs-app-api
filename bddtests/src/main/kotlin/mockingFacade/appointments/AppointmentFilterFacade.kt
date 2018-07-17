package mockingFacade.appointments

data class AppointmentFilterFacade(
        val type: String ,
        var location: String,
        var doctor: String? = null,
        var date: String? = null
)
