package mocking.emis.models

data class Appointment(
        val slotId: Int,
        val sessionId: Int = 1,
        var startTime: String = "",
        var endTime: String = "",
        var bookingDate: String = "",
        val bookingReason: String = "",
        var BookingMethod: String = "PatientFacingServices",
        var slotTypeName: String = "Default",
        var slotTypeStatus: SlotTypeStatus = SlotTypeStatus.Practice,
        var vidyoRoomUri: String? = null,
        var telephoneAppointmentDetails: TelephoneAppointmentDetails? = null
)
