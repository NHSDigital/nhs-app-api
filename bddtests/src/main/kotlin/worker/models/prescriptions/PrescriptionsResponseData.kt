package worker.models.prescriptions

import worker.models.courses.Course

data class PrescriptionsResponseData(val prescriptions: List<PrescriptionItem>,
                                     val courses: List<Course>)
