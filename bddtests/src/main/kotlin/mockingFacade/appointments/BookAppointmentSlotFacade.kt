package mockingFacade.appointments

class BookAppointmentSlotFacade(val userPatientLinkToken: String,
                                val slotId: Int,
                                val bookingReason: String? = null,
                                val startTime: String? = null,
                                val endTime: String? = null,
                                val telephoneNumber: String? = null,
                                val telephoneContactType: String? = null)