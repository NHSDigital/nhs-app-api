package worker.models.appointments

class CancelAppointmentRequest(val appointmentId: String, val cancellationReasonId: String)