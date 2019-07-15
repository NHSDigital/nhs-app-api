package worker.models.appointments

import mocking.emis.models.SlotTypeStatus

data class AppointmentBookRequest(
        var userPatientLinkToken: String? = null,
        var slotId: String? = null,
        var bookingReason: String? = null,
        var startTime: String? =null,
        var endTime: String? = null,
        var telephoneNumber: String? = null,
        var telephoneContactType: String? = null,
        var channel: SlotTypeStatus = SlotTypeStatus.Unknown
)