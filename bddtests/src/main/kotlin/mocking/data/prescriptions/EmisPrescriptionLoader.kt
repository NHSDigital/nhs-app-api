package mocking.data.prescriptions

import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.emis.models.PrescriptionRequest
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourse
import mocking.emis.models.RequestedMedicationCourseStatus
import models.prescriptions.MedicationCourse
import models.prescriptions.PrescriptionLoaderConfiguration
import java.time.OffsetDateTime

object EmisPrescriptionLoader : IPrescriptionLoader<PrescriptionRequestsGetResponse> {
    override lateinit var data: PrescriptionRequestsGetResponse

    private const val MAX_RANDOM_NUMBER = 6
    private var requestedByDisplayName = ""
    private var requestedByForenames = ""
    private var requestedBySurname = ""

    override fun loadData(prescriptionLoaderConfig: PrescriptionLoaderConfiguration,
                          prescriptionCompletedByProxy: Boolean) {

        val prescriptionRequests = mutableListOf<PrescriptionRequest>()
        var medicationCourses = mutableListOf<MedicationCourse>()

        getValuesForRequestedByFields(prescriptionCompletedByProxy)

        if (prescriptionLoaderConfig.noPrescriptions != 0) {
            // Create courses first as these will be used in the prescriptions
            EmisCoursesLoader.loadData(
                    maxCourses = prescriptionLoaderConfig.noCourses,
                    numOfRepeats = prescriptionLoaderConfig.noRepeats,
                    numCanBeRequested = prescriptionLoaderConfig.noRepeats,
                    includeDosage = prescriptionLoaderConfig.showDosage,
                    includeQuantity = prescriptionLoaderConfig.showQuantity)

            medicationCourses = medicationCourses.union(EmisCoursesLoader.data).toMutableList()

            var maxNumberOfPrescriptions = prescriptionLoaderConfig.noPrescriptions.minus(1)
            var isSecondIteration = false
            var prescriptionNumber = 0
            var courseNumber = medicationCourses.count().minus(1)

            while (prescriptionNumber <= maxNumberOfPrescriptions) {
                val requestedMedicationCourses = mutableListOf<RequestedMedicationCourse>()
                var time = OffsetDateTime.now()

                time = time.minusDays(prescriptionNumber.toLong())

                if (!isSecondIteration) {
                    requestedMedicationCourses.add(
                            RequestedMedicationCourse(
                                    medicationCourses.get(courseNumber).medicationCourseGuid,
                                    RequestedMedicationCourseStatus.Requested))

                    prescriptionRequests.add(
                            PrescriptionRequest(
                                    time.toString(), requestedMedicationCourses, getPrescriptionStatus().toString(),
                                    requestedByDisplayName, requestedByForenames,
                                    requestedBySurname))
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
                    maxNumberOfPrescriptions = prescriptionLoaderConfig.noPrescriptions.minus(1)
                    prescriptionNumber = 0
                } else if (prescriptionNumber < maxNumberOfPrescriptions && courseNumber == -1) {
                    prescriptionNumber++
                    courseNumber = medicationCourses.count().minus(1)
                } else {
                    prescriptionNumber++
                }
            }
        }

        data = PrescriptionRequestsGetResponse(prescriptionRequests, medicationCourses)
    }

    private fun getValuesForRequestedByFields(completedByProxy: Boolean){
        if(completedByProxy){
            requestedByDisplayName = "Completed by proxy"
            requestedByForenames = "Main"
            requestedBySurname = "user"
        }
        else {
            requestedByDisplayName = "Completed by main user"
            requestedByForenames = "Completed by"
            requestedBySurname = "main user"
        }
    }

    private fun getPrescriptionStatus(): RequestedMedicationCourseStatus {
        return RequestedMedicationCourseStatus.values()[getRandomNumber(MAX_RANDOM_NUMBER)]
    }

    fun orderCourses(orderedCourses: MutableList<MedicationCourse>,
                     oldPrescriptions: PrescriptionRequestsGetResponse = data)
            : PrescriptionRequestsGetResponse {

        //1 create new prescription object and add ordered courses to it
        val newCourses = mutableListOf<RequestedMedicationCourse>()
        orderedCourses.forEach { course ->
            newCourses.add(
                    RequestedMedicationCourse(
                            course.medicationCourseGuid, RequestedMedicationCourseStatus.Requested))
        }

        val prescriptionsList = mutableListOf<PrescriptionRequest>()
        prescriptionsList.add(PrescriptionRequest(OffsetDateTime.now().toString(),
                newCourses, RequestedMedicationCourseStatus.Requested.toString(),
                "Ordered by main", "Ordered by",
                "main"))

        //2 add all the old prescriptions to the list
        oldPrescriptions.prescriptionRequests.forEach { pr ->
            prescriptionsList.add(pr)
        }

        //3 update course list
        val courseList = mutableSetOf<MedicationCourse>()
        oldPrescriptions.medicationCourses.forEach { c ->
            courseList.add(c)
        }
        orderedCourses.forEach { c ->
            courseList.add(MedicationCourse(
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
