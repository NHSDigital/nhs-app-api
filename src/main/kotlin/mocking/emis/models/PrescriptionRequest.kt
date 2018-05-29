package mocking.emis.models

data class PrescriptionRequest(val DateRequested: String,
                               var requestedMedicationCourses: MutableList<RequestedMedicationCourse>)
