package mocking.emis.models

import models.prescriptions.MedicationCourse

data class PrescriptionRequestsGetResponse(
        var prescriptionRequests: List<PrescriptionRequest>,
        var medicationCourses: List<MedicationCourse>)
