package mockingFacade.appointments

import mocking.data.appointments.FilterSlotDetails

data class AppointmentFilterFacade(
        val type: String? = null,
        val location: String? = null,
        val doctor: String? = null,
        val filteredSlots: Map<String, Set<FilterSlotDetails>> = mapOf()
)
