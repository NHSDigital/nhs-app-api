package mocking.emis.appointments

class CancelAppointmentRequest(val UserPatientLinkToken: String, val SlotId: String, val CancellationReason: String)