package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.defaults.dataPopulation.journies.courses.CoursesDataBuilder
import mocking.emis.models.PrescriptionRequest
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourse
import mocking.emis.models.RequestedMedicationCourseStatus
import models.prescriptions.MedicationCourse
import java.time.OffsetDateTime
import java.util.*

object PrescriptionsData {

    fun loadPrescriptionsData(noPrescriptions: Int, noCourses: Int, noRepeats: Int?,
                              showDosage: Boolean = true, showQuantity: Boolean = true):
            PrescriptionRequestsGetResponse {
        val prescriptionRequests = mutableListOf<PrescriptionRequest>()
        var medicationCourses = mutableListOf<MedicationCourse>()

        if (noPrescriptions != 0) {

            medicationCourses =
                    CoursesDataBuilder()
                            .maxCourses(noCourses)
                            .numOfRepeats(noRepeats ?: noPrescriptions)
                            .numCanBeRequested(noRepeats ?: noPrescriptions)
                            .medicationCourses(medicationCourses)
                            .includeDosage(showDosage)
                            .includeQuantity(showQuantity)
                            .build()

            var maxPrescriptions = noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNum = 0
            var courseNum = medicationCourses.count().minus(1)

            while (prescriptionNum <= maxPrescriptions) {
                val requestedMedicationCourses = mutableListOf<RequestedMedicationCourse>()
                var time = OffsetDateTime.now()

                time = time.minusDays(prescriptionNum.toLong())

                if (!isSecondIteration) {
                    requestedMedicationCourses.add(RequestedMedicationCourse(
                            medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))
                    prescriptionRequests.add(PrescriptionRequest(
                            time.toString(), requestedMedicationCourses,
                            RequestedMedicationCourseStatus.Requested.toString()))
                } else {
                    requestedMedicationCourses.add(RequestedMedicationCourse(
                            medicationCourses.get(courseNum).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))

                    prescriptionRequests.get(
                            prescriptionNum).requestedMedicationCourses.addAll(requestedMedicationCourses)
                }

                courseNum--

                if (prescriptionNum == maxPrescriptions && courseNum >= 0) {
                    isSecondIteration = true
                    maxPrescriptions = noPrescriptions.minus(1)
                    prescriptionNum = 0
                } else if (prescriptionNum < maxPrescriptions && courseNum == -1) {
                    prescriptionNum++
                    courseNum = medicationCourses.count().minus(1)
                } else {
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

    private fun getStringValue(list: List<String>): String {
        return list.get(getRandomNumber(getMedicationCourseNames().size))
    }

    private fun getMedicationCourseNames(): List<String> {
        return listOf(
                "Ranitidine 150mg effervescent tablets",
                "Codine 200mg tablets",
                "Choline salicylate 8.7% oromucosal gel sugar free",
                "Paracetamol 150mg oral tablets",
                "Penicillin 150mg oral tablets"
        )
    }

    private fun getDosages(): List<String> {
        return listOf(
                "One To Be Taken Twice A Day",
                "One To Be Taken Three Times A Day",
                "One To Be Taken Weekly",
                "Two To Be Taken Four Times A Day",
                "One To Be Take Every Evening"
        )
    }

    fun getQuantity(quantity: Int): String {
        val list = listOf(
                "$quantity gram",
                "$quantity tablet",
                "$quantity ml")

        return list.get(getRandomNumber(list.size))
    }

    fun getRandomNumber(maxNum: Int): Int {
        val random = Random()
        val minNum = 1

        var localMaxNum = maxNum

        if (localMaxNum == 1) {
            localMaxNum += 1
        }

        return random.nextInt(localMaxNum - minNum) + minNum
    }

    fun addResponses(list: List<PrescriptionRequestsGetResponse>): PrescriptionRequestsGetResponse {

        val returnPrescriptions = mutableListOf<PrescriptionRequest>()
        val returnCourses = mutableSetOf<MedicationCourse>()

        list.forEach { el ->
            el.prescriptionRequests.forEach { pr ->
                returnPrescriptions.add(pr)
            }

            el.medicationCourses.forEach { mr ->
                returnCourses.add(mr)
            }
        }
        return PrescriptionRequestsGetResponse(returnPrescriptions, returnCourses.toList())
    }
}
