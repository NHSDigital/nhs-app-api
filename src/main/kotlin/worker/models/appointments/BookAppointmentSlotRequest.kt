package worker.models.appointments

class BookAppointmentSlotRequest(val userPatientLinkToken: String, val slotId: Int, val bookingReason: String)