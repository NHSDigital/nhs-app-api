package worker.models.appointments

data class AppointmentSlotsResponse(
        var slots: Array<SlotResponseObject>,
        var telephoneNumbers: Array<TelephoneNumber>
)
