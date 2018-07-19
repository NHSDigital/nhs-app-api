package mockingFacade.appointments

import mocking.emis.models.SlotTypeStatus

data class AppointmentSlotFacade(
        val slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var slotTypeName: String? = null,
        var slotTypeStatus: SlotTypeStatus? = null
)
