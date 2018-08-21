package mocking.emis.models

data class InputRequirements(
        var appointmentBookingReason: String = "RequestedOptional",
        var prescribingComment: String = "RequestedOptional"
)
