package mocking.emis.models

import models.Channel

data class AppointmentSlot(
        val slotId: Int,
        var startTime: String? = null,
        var endTime: String? = null,
        var slotTypeName: String? = null,
        var slotTypeStatus: Channel = Channel.Unknown
)