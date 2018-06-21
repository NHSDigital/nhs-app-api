package mocking.emis.models

data class PrescriptionRequest(var dateRequested: String,
                               var requestedMedicationCourses: MutableList<RequestedMedicationCourse>,
                               var status: String)
