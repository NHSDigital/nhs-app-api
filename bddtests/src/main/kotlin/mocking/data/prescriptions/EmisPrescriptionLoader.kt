package mocking.data.prescriptions

import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.emis.models.PrescriptionRequest
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourse
import mocking.emis.models.RequestedMedicationCourseStatus
import models.prescriptions.MedicationCourse
import java.time.OffsetDateTime

object EmisPrescriptionLoader : IPrescriptionLoader<PrescriptionRequestsGetResponse> {
    override lateinit var data:PrescriptionRequestsGetResponse

    private const val MAX_RANDOM_NUMBER = 6

    override fun loadData(noPrescriptions: Int, noCourses: Int,
                          noRepeats: Int, showDosage: Boolean,
                          showQuantity: Boolean) {

        val prescriptionRequests = mutableListOf<PrescriptionRequest>()
        var medicationCourses = mutableListOf<MedicationCourse>()

        if(noPrescriptions != 0) {
            // Create courses first as these will be used in the prescriptions
            EmisCoursesLoader.loadData(
                    maxCourses = noCourses,
                    numOfRepeats = noRepeats,
                    numCanBeRequested = noRepeats,
                    includeDosage = showDosage,
                    includeQuantity = showQuantity)

            medicationCourses = medicationCourses.union(EmisCoursesLoader.data).toMutableList()

            var maxNumberOfPrescriptions = noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNumber = 0
            var courseNumber = medicationCourses.count().minus(1)

            while (prescriptionNumber <= maxNumberOfPrescriptions) {
                var requestedMedicationCourses = mutableListOf<RequestedMedicationCourse>()
                var time = OffsetDateTime.now()

                time = time.minusDays(prescriptionNumber.toLong())

                if (!isSecondIteration) {
                    requestedMedicationCourses.add(
                            RequestedMedicationCourse(
                                    medicationCourses.get(courseNumber).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))
                    prescriptionRequests.add(
                            PrescriptionRequest(
                                    time.toString(), requestedMedicationCourses, getPrescriptionStatus().toString()))
                } else {
                    requestedMedicationCourses.add(
                            RequestedMedicationCourse(medicationCourses.get(courseNumber).medicationCourseGuid,
                            RequestedMedicationCourseStatus.Requested))

                    prescriptionRequests.get(
                            prescriptionNumber).requestedMedicationCourses.addAll(requestedMedicationCourses)
                }

                courseNumber--

                if (prescriptionNumber == maxNumberOfPrescriptions && courseNumber >= 0) {
                    isSecondIteration = true
                    maxNumberOfPrescriptions = noPrescriptions.minus(1)
                    prescriptionNumber = 0
                } else if (prescriptionNumber < maxNumberOfPrescriptions && courseNumber == -1) {
                    prescriptionNumber++
                    courseNumber = medicationCourses.count().minus(1)
                }
                else{
                    prescriptionNumber++
                }
            }
        }

        data = PrescriptionRequestsGetResponse(prescriptionRequests, medicationCourses)
    }

    private fun getPrescriptionStatus(): RequestedMedicationCourseStatus {
        return RequestedMedicationCourseStatus.values()[getRandomNumber(MAX_RANDOM_NUMBER)]
    }

    fun orderCourses(orderedCourses: MutableList<MedicationCourse>,
                     oldPrescriptions: PrescriptionRequestsGetResponse = data)
            : PrescriptionRequestsGetResponse {

        //1 create new prescription object and add ordered courses to it..
        val newCourses = mutableListOf<RequestedMedicationCourse>()
        orderedCourses.forEach {
            course -> newCourses.add(
                RequestedMedicationCourse(
                        course.medicationCourseGuid, RequestedMedicationCourseStatus.Requested))
        }

        val prescriptionsList = mutableListOf<PrescriptionRequest>()
        prescriptionsList.add(PrescriptionRequest(OffsetDateTime.now().toString(),
                                                  newCourses, RequestedMedicationCourseStatus.Requested.toString()))

        //2 add all the old prescriptions to the list
        oldPrescriptions.prescriptionRequests.forEach {
            pr -> prescriptionsList.add(pr)
        }

        //3 update course list
        val courseList = mutableSetOf<MedicationCourse>()
        oldPrescriptions.medicationCourses.forEach {
            c -> courseList.add(c)
        }
        orderedCourses.forEach {
            c -> courseList.add(MedicationCourse(
                c.medicationCourseGuid,
                c.name,
                c.dosage,
                c.quantityRepresentation,
                c.prescriptionType,
                c.constituents,
                c.canBeRequested))
        }

        return PrescriptionRequestsGetResponse(prescriptionsList, courseList.toList())
    }
}
