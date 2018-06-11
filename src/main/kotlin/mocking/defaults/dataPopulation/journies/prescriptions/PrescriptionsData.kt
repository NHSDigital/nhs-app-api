package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.defaults.dataPopulation.journies.courses.CoursesData
import mocking.emis.models.*
import java.time.OffsetDateTime
import java.util.*

object PrescriptionsData {

    fun loadPrescriptionsData(noPrescriptions: Int, noCourses: Int, noRepeats: Int?): PrescriptionRequestsGetResponse {

        var prescriptionRequests = mutableListOf<PrescriptionRequest>()
        var medicationCourses = mutableListOf<MedicationCourse>()

        if(noPrescriptions != 0) {

            // Create courses first as these will be used in the prescriptions
            medicationCourses = CoursesData.getCourseData(noCourses, if (noRepeats == null) noPrescriptions else noRepeats!!, if (noRepeats == null) noPrescriptions else noRepeats, medicationCourses)

            var maxPrescriptions = noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNum = 0
            var courseNum = medicationCourses.count().minus(1)

            while (prescriptionNum <= maxPrescriptions) {
                var requestedMedicationCourses = mutableListOf<RequestedMedicationCourse>()
                var time = OffsetDateTime.now()
                time = time.plusSeconds(1)

                if (!isSecondIteration) {
                    requestedMedicationCourses.add(RequestedMedicationCourse(medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))
                    prescriptionRequests.add(PrescriptionRequest(time.toString(), requestedMedicationCourses))
                } else {
                    requestedMedicationCourses.add(RequestedMedicationCourse(medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))

                    prescriptionRequests.get(prescriptionNum).requestedMedicationCourses.addAll(requestedMedicationCourses)
                }

                courseNum--

                if (prescriptionNum == maxPrescriptions && courseNum > 0) {
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

        return PrescriptionRequestsGetResponse(prescriptionRequests, medicationCourses)
    }

    fun getCourseName(): String {
        return getStringValue(getMedicationCourseNames())
    }

    fun getDosage(): String {
        return getStringValue(getDosages())
    }

    fun getStringValue(list: List<String>): String {
        return list.get(getrandomNumber(getMedicationCourseNames().size))
    }

    fun getMedicationCourseNames(): List<String> {
        return listOf(
                "Ranitidine 150mg effervescent tablets",
                "Codine 200mg tablets",
                "Choline salicylate 8.7% oromucosal gel sugar free",
                "Paracetamol 150mg oral tablets",
                "Penicillin 150mg oral tablets"
        )
    }

    fun getDosages(): List<String> {
        return listOf(
                "One To Be Taken Twice A Day",
                "One To Be Taken Three Times A Day",
                "One To Be Taken Weekly",
                "Two To Be Taken Four Times A Day",
                "One To Be Take Every Evening"
        )
    }

    fun getQuatity(): String {

        val quantity = getrandomNumber(100)
        val list = listOf(
                "$quantity gram",
                "$quantity tablet",
                "$quantity  ml")

        return list.get(getrandomNumber(list.size))
    }

    fun getrandomNumber(maxNum: Int): Int {
        val random = Random()
        var minNum = 1

        var localMaxNum = maxNum

        if(localMaxNum == 1){
            localMaxNum += 1
        }

        return random.nextInt(localMaxNum - minNum) + minNum
    }
}
