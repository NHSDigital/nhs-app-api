package mocking.emis.models

data class AppointmentSlot(
        val slotId: Int,
        var startTime: String? = null,
        var endTime: String? = null,
        var slotTypeName: String? = null,
        var slotTypeStatus: SlotTypeStatus = SlotTypeStatus.Unknown,
        var telephoneNumber: String
)