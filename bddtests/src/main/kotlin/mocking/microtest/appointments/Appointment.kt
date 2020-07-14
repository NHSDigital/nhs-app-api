package mocking.microtest.appointments

class Appointment (

    val id: String,
    val type: String,
    val startTime: String,
    val duration: String,
    val endTime: String,
    val location: String,
    val clinicians: List<String>,
    val channel: String,
    val telephoneNumber: String
)
