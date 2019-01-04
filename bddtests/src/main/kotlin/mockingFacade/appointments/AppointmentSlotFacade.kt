package mockingFacade.appointments

import models.Channel

data class AppointmentSlotFacade(
        val slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var sessionTypeName: String? = null,
        var slotTypeName: String? = "Slot",
        var slotTypeId: Int? = 1,
        var slotInThePast: Boolean? = false,
        var channel: Channel = Channel.Unknown
)
