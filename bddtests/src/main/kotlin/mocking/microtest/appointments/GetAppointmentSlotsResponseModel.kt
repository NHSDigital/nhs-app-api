package mocking.microtest.appointments

data class GetAppointmentSlotsResponseModel(
        var slots: List<AppointmentSlot> = emptyList()
)
