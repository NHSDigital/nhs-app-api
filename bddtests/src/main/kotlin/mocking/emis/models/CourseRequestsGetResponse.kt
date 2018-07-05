package mocking.emis.models

import models.prescriptions.MedicationCourse

data class CourseRequestsGetResponse (
        val courses: List<MedicationCourse>)
