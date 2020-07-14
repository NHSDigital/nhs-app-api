package mockingFacade.appointments

class CancelAppointmentSlotFacade(val userPatientLinkToken: String,
                                  val slotId: Int,
                                  val cancellationReason: String)
