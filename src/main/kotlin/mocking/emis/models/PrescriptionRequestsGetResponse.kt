package mocking.emis.models

data class PrescriptionRequestsGetResponse(
        var prescriptionRequests: List<PrescriptionRequest>,
        var medicationCourses: List<MedicationCourse>)
