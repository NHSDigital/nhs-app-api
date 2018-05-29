package mocking.emis.models

data class PrescriptionRequestsGetResponse(
        val prescriptionRequests: List<PrescriptionRequest>,
        val medicationCourses: List<MedicationCourse>)
