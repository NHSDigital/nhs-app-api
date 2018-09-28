package mocking.stubs.appointments

class AppointmentMatchers {
    companion object {
        const val appointmentBookingSlotForMatcher = 301
        const val cancellationSlotMatcher = 1
        const val appointmentNotFoundMatcher = "give me an appointment not found response"
        const val successMatcherForAppointments = "give me a good response"
        const val cancellationReasonMatcher = "No longer required"
        const val timeoutMatcherForAppointments = "give me a time out response"
    }
}