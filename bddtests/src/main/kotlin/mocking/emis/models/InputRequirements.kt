package mocking.emis.models

import mocking.emis.practices.NecessityOption

data class InputRequirements(
        var appointmentBookingReason: String = NecessityOption.OPTIONAL.text,
        var prescribingComment: String = NecessityOption.OPTIONAL.text
)
