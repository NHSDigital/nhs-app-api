package worker.models.appointments

data class AppointmentSlotsResponse(
        var clinicians: Array<GenericResponseObject>,
        var appointmentSessions: Array<GenericResponseObject>,
        var locations: Array<GenericResponseObject>,
        var slots: Array<SlotResponseObject>
)