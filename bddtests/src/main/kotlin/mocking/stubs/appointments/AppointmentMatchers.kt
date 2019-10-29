package mocking.stubs.appointments

class AppointmentMatchers {
    companion object {
        const val appointmentBookingSlotForMatcher = 100 
        const val cancellationSlotMatcher = 1
        const val appointmentNotFoundMatcher = "give me an appointment not found response"
        const val successMatcherForAppointments = "give me a good response"
        const val cancellationReasonMatcher = "No longer required"
        const val timeoutMatcherForAppointments = "give me a time out response"
        const val appointmentConflictMatcher = "give me an appointment conflict response"
        const val appointmentLimitReachedMatcher = "give me an appointment limit reached response"
    }
}