package mocking.emis.models

data class PrescriptionRequest(var DateRequested: String,
                               var requestedMedicationCourses: MutableList<RequestedMedicationCourse>)
