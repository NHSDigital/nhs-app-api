package features.prescriptions.mappers

import constants.DateTimeFormats
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.PrescriptionType
import mocking.emis.models.RequestedMedicationCourseStatus
import models.prescriptions.HistoricPrescription
import models.prescriptions.MedicationCourse
import org.joda.time.DateTime

private const val REQUESTED_PRIORITY = 1
private const val APPROVED_PRIORITY = 2
private const val REJECTED_PRIORITY = 3
private const val UPPER_LIMIT_FOR_TOTAL_COURSES = 100

object EmisPrescriptionMapper {

    fun map(data: PrescriptionRequestsGetResponse): List<HistoricPrescription> {

        val historicPrescriptionOrderPriority = hashMapOf(
                "Requested" to REQUESTED_PRIORITY,
                "Approved" to APPROVED_PRIORITY,
                "Rejected" to REJECTED_PRIORITY)

        val displayedEmisMedicationCourseStatuses = listOf(
                RequestedMedicationCourseStatus.Rejected,
                RequestedMedicationCourseStatus.Requested,
                RequestedMedicationCourseStatus.ForwardedForSigning,
                RequestedMedicationCourseStatus.Issued)

        val emisMedicationCourseStatusToDisplayedStatus = mapOf(
                RequestedMedicationCourseStatus.Requested to "Requested",
                RequestedMedicationCourseStatus.ForwardedForSigning to "Requested",
                RequestedMedicationCourseStatus.Issued to "Approved",
                RequestedMedicationCourseStatus.Rejected to "Rejected")

        var totalCoursesRunningTotal = 0

        val repeatCourses = data.medicationCourses.filter { it.prescriptionType == PrescriptionType.Repeat }

        val repeatCourseguids = getGuids(repeatCourses)

        val historicPrescriptions = ArrayList<HistoricPrescription>()

        for (prescription in data.prescriptionRequests.toList().sortedByDescending { it.dateRequested }){

            if (totalCoursesRunningTotal >= UPPER_LIMIT_FOR_TOTAL_COURSES) {
                break
            }

            val datetime = DateTime.parse(prescription.dateRequested).toString(DateTimeFormats.frontendDateFormat)

            val filteredCoursesInPrescription = prescription.requestedMedicationCourses.filter {
                it -> repeatCourseguids.contains(it.requestedMedicationCourseGuid)
                    && it.requestedMedicationCourseStatus in displayedEmisMedicationCourseStatuses
            }

            for (courseEntry in filteredCoursesInPrescription) {

                val course =
                        repeatCourses.toList().filter { it.medicationCourseGuid ==
                                courseEntry.requestedMedicationCourseGuid }.single()

                val historicPrescription = HistoricPrescription(
                        name = course.name,
                        dosage = pages.prescription.resolveDetailsField(course.dosage,
                                course.quantityRepresentation).joinToString(" - ")
                )
                historicPrescription.orderDate = datetime
                historicPrescription.status =
                        emisMedicationCourseStatusToDisplayedStatus[courseEntry.requestedMedicationCourseStatus]

                historicPrescriptions.add(historicPrescription)
            }

            totalCoursesRunningTotal += filteredCoursesInPrescription.size
        }

        val historicPrescriptionsOrderedByStatusOnScreen =
                historicPrescriptions.sortedWith(compareBy({ historicPrescriptionOrderPriority[it.status] }))

        return historicPrescriptionsOrderedByStatusOnScreen
    }

    private fun getGuids(repeatCourses: List<MedicationCourse>): ArrayList<String> {
        val courseGuids = ArrayList<String>()

        repeatCourses.forEach { it -> courseGuids.add(it.medicationCourseGuid) }

        return courseGuids
    }

}
