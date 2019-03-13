package mockingFacade.appointments

import mocking.emis.models.SlotTypeStatus

data class AppointmentSlotFacade(
        val slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var sessionTypeName: String? = null,
        var slotTypeName: String? = "Slot",
        var slotTypeId: Int? = 1,
        var slotInThePast: Boolean? = false,
        var channel: SlotTypeStatus = SlotTypeStatus.Unknown
)
