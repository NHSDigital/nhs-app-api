package mocking.data.prescriptions

import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.emis.models.*
import models.prescriptions.MedicationCourse
import java.time.OffsetDateTime

object EmisPrescriptionLoader : IPrescriptionLoader<PrescriptionRequestsGetResponse> {
    override lateinit var data:PrescriptionRequestsGetResponse

    override fun loadData(noPrescriptions: Int, noCourses: Int, noRepeats: Int, showDosage: Boolean, showQuantity: Boolean) {

        val prescriptionRequests = mutableListOf<PrescriptionRequest>()
        var medicationCourses = mutableListOf<MedicationCourse>()

        if(noPrescriptions != 0) {

            // Create courses first as these will be used in the prescriptions
            EmisCoursesLoader.loadData(
                    noCourses,
                    noRepeats,
                    noRepeats,
                    showDosage,
                    showQuantity)

            medicationCourses = medicationCourses.union(EmisCoursesLoader.data).toMutableList()

            var maxPrescriptions = noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNum = 0
            var courseNum = medicationCourses.count().minus(1)

            while (prescriptionNum <= maxPrescriptions) {
                var requestedMedicationCourses = mutableListOf<RequestedMedicationCourse>()
                var time = OffsetDateTime.now()

                time = time.minusDays(prescriptionNum.toLong())

                if (!isSecondIteration) {
                    requestedMedicationCourses.add(RequestedMedicationCourse(medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))
                    prescriptionRequests.add(PrescriptionRequest(time.toString(), requestedMedicationCourses, getPrescriptionStatus().toString()))
                } else {
                    requestedMedicationCourses.add(RequestedMedicationCourse(medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))

                    prescriptionRequests.get(prescriptionNum).requestedMedicationCourses.addAll(requestedMedicationCourses)
                }

                courseNum--

                if (prescriptionNum == maxPrescriptions && courseNum >= 0) {
                    isSecondIteration = true
                    maxPrescriptions = noPrescriptions.minus(1)
                    prescriptionNum = 0
                } else if (prescriptionNum < maxPrescriptions && courseNum == -1) {
                    prescriptionNum++
                    courseNum = medicationCourses.count().minus(1)
                }
                else{
                    prescriptionNum++
                }
            }
        }

        data = PrescriptionRequestsGetResponse(prescriptionRequests, medicationCourses)
    }





    private fun getPrescriptionStatus(): RequestedMedicationCourseStatus {
        return RequestedMedicationCourseStatus.values()[getRandomNumber(6)]
    }


}
