package mockingFacade.appointments

class BookAppointmentSlotFacade(val userPatientLinkToken: String,
                                val slotId: Int,
                                val bookingReason: String,
                                val startTime: String? = null,
                                val endTime: String? = null)