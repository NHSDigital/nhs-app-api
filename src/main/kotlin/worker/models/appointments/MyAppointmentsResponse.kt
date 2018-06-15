package worker.models.appointments

data class MyAppointmentsResponse(
        var appointments: ArrayList<AppointmentResponseObject> = arrayListOf(),
        var clinicians: ArrayList<GenericResponseObject> = arrayListOf(),
        var appointmentSessions: ArrayList<GenericResponseObject> = arrayListOf(),
        var locations: ArrayList<GenericResponseObject> = arrayListOf(),
        var cancellationReasons: ArrayList<GenericResponseObject> = arrayListOf()
)