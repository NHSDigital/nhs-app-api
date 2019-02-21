package worker.models.appointments

data class MyAppointmentsResponse(
        var upcomingAppointments: List<AppointmentResponseObject> = arrayListOf(),
        var pastAppointments: List<AppointmentResponseObject> = arrayListOf(),
        var cancellationReasons: List<GenericResponseObject> = arrayListOf()
)