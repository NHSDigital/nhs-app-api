package mocking.emis.models

data class AppointmentSlot(
        var slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var slotTypeName: String? = null,
        var slotTypeStatus: SlotTypeStatus? = null
)