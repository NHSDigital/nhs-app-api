package mocking.emis.appointments

class CancelAppointmentRequest(val UserPatientLinkToken: String, val SlotId: Int, val CancellationReason: String)