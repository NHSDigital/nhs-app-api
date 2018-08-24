package mockingFacade.appointments

data class AppointmentFilterFacade(
        val type: String? = null,
        var location: String? = null,
        var doctor: String? = null,
        var date: String? = null,
        var filteredSlots: Map<String, Set<String>> = mapOf()
)
