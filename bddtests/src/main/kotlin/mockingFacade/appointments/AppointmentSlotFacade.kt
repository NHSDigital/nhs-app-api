package mockingFacade.appointments

import mocking.emis.models.SlotTypeStatus

data class AppointmentSlotFacade(
        var slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var slotTypeId: Int = 1,
        var slotInThePast: Boolean? = false,
        var channel: SlotTypeStatus = SlotTypeStatus.Unknown,
        var slotDetails: String = "",
        var telephoneNumber: String = ""
)
