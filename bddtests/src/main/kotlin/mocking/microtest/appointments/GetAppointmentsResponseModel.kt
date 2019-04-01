package mocking.microtest.appointments

data class GetAppointmentsResponseModel (
    var appointments: List<Appointment> = emptyList()
)