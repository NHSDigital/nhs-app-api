package mockingFacade.appointments

class MyAppointmentsFacade (
        var appointmentsFromDateTime: String,
        val slots: AppointmentSlotsResponseFacade? = null
)