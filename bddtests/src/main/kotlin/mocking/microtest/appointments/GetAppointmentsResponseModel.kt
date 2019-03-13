package mocking.microtest.appointments

data class GetAppointmentsResponseModel(
        var slots: List<AppointmentSlot> = emptyList()
)
