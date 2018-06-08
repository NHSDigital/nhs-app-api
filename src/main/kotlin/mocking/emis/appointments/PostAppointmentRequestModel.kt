package mocking.emis.appointments

class PostAppointmentRequestModel(
    var userPatientLinkToken: String,
    var slotId: Int,
    var bookingReason: String
)