package worker.models.appointments

data class MyAppointmentsResponse(
        var appointments: List<AppointmentResponseObject> = arrayListOf(),
        var cancellationReasons: List<GenericResponseObject> = arrayListOf()
)