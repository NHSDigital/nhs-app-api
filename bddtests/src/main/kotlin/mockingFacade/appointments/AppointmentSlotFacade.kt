package mockingFacade.appointments

data class AppointmentSlotFacade(
        val slotId: Int? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var sessionTypeName: String? = null,
        var slotTypeName: String? = "Slot",
        var slotTypeId: Int? = 1,
        var slotInThePast: Boolean? = false
)
