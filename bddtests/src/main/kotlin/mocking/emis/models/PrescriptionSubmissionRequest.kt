package mocking.emis.models

data class PrescriptionSubmissionRequest(val MedicationCourseGuids: List<String>? = null,
                                         val RequestComment: String? = null,
                                         var UserPatientLinkToken: String? = null)
