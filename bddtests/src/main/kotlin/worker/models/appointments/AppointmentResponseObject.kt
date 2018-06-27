package worker.models.appointments

data class AppointmentResponseObject (
        val id:String,
        val startTime:String,
        val endTime:String,
        val locationId:String,
        val appointmentSessionId:String,
        val clinicianIds:ArrayList<String>
)