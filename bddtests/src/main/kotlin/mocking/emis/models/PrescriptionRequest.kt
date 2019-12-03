package mocking.emis.models

data class PrescriptionRequest(var dateRequested: String,
                               var requestedMedicationCourses: MutableList<RequestedMedicationCourse>,
                               var status: String,
                               var requestedByDisplayName: String,
                               var requestedByForenames: String,
                               var requestedBySurname: String)
