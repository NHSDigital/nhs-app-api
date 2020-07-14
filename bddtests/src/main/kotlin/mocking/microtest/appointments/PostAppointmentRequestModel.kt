package mocking.microtest.appointments

class PostAppointmentRequestModel(
    var slotId: String,
    var bookingReason: String,
    var telephoneNumber: String?
)
