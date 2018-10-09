package mocking.emis.models

import mocking.emis.practices.PrescribingComment

data class InputRequirements(
        var appointmentBookingReason: String = "RequestedOptional",
        var prescribingComment: String = PrescribingComment.REQUESTED_OPTIONAL
)
