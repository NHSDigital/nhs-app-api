package features.prescriptions.mappers

import models.prescriptions.MedicationCourse
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.PrescriptionType
import mocking.emis.models.RequestedMedicationCourseStatus
import models.prescriptions.HistoricPrescription
import org.joda.time.DateTime

object EmisPrescriptionMapper {

    fun Map(data: PrescriptionRequestsGetResponse): List<HistoricPrescription> {

        val historicPrescriptionOrderPriority = hashMapOf( "Rejected" to 1, "Requested" to 2, "Approved" to 3)

        val displayedEmisMedicationCourseStatuses = listOf(
                RequestedMedicationCourseStatus.Rejected,
                RequestedMedicationCourseStatus.Requested,
                RequestedMedicationCourseStatus.ForwardedForSigning,
                RequestedMedicationCourseStatus.Issued)

        val emisMedicationCourseStatusToDisplayedStatus = mapOf(
                RequestedMedicationCourseStatus.Rejected to "Rejected",
                RequestedMedicationCourseStatus.Requested to "Requested",
                RequestedMedicationCourseStatus.ForwardedForSigning to "Requested",
                RequestedMedicationCourseStatus.Issued to "Approved")

        var totalCoursesRunningTotal = 0

        var repeatCourses = data.medicationCourses.filter { it.prescriptionType == PrescriptionType.Repeat }

        var repeatCourseguids = GetGuids(repeatCourses)

        var historicPrescriptions = ArrayList<HistoricPrescription>()

        for (prescription in data.prescriptionRequests.toList().sortedByDescending { it.dateRequested }){

            if (totalCoursesRunningTotal >= 100) {
                break
            }

            var datetime = DateTime.parse(prescription.dateRequested).toString("d MMM yyyy")

            var filteredCoursesInPrescription = prescription.requestedMedicationCourses.filter {
                it -> repeatCourseguids.contains(it.requestedMedicationCourseGuid)
                    && it.requestedMedicationCourseStatus in displayedEmisMedicationCourseStatuses
            }

            for (courseEntry in filteredCoursesInPrescription) {

                var course = repeatCourses.toList().filter { it.medicationCourseGuid == courseEntry.requestedMedicationCourseGuid }.single()

                var historicPrescription = HistoricPrescription(
                        name = course.name,
                        dosage = pages.prescription.resolveDetailsField(course.dosage, course.quantityRepresentation)
                )
                historicPrescription.orderDate = datetime
                historicPrescription.status = emisMedicationCourseStatusToDisplayedStatus[courseEntry.requestedMedicationCourseStatus]

                historicPrescriptions.add(historicPrescription)
            }

            totalCoursesRunningTotal += filteredCoursesInPrescription.size
        }

        val historicPrescriptionsOrderedByStatusOnScreen =
                historicPrescriptions.sortedWith(compareBy({ historicPrescriptionOrderPriority[it.status] }))

        return historicPrescriptionsOrderedByStatusOnScreen
    }

    private fun GetGuids(repeatCourses: List<MedicationCourse>): ArrayList<String> {
        var courseGuids = ArrayList<String>()

        repeatCourses.forEach { it -> courseGuids.add(it.medicationCourseGuid) }

        return courseGuids
    }

}