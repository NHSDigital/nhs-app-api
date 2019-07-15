package mockingFacade.appointments

data class BookAppointmentSlotFacade(val userPatientLinkToken: String? = null,
                                val slotId: Int,
                                val bookingReason: String? = null,
                                val startTime: String? = null,
                                val endTime: String? = null,
                                val channel: String? = null,
                                val telephoneNumber: String? = null,
                                val telephoneContactType: String? = null)