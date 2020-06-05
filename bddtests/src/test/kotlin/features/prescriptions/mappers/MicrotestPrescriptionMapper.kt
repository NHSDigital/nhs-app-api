package features.prescriptions.mappers

import constants.DateTimeFormats
import mocking.microtest.prescriptions.CourseStatus
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.microtest.prescriptions.PrescriptionStatus
import models.prescriptions.HistoricPrescription
import org.joda.time.DateTime

private const val REJECTED_PRIORITY = 1
private const val REQUESTED_PRIORITY = 2
private const val APPROVED_PRIORITY = 3
private const val UPPER_LIMIT_FOR_TOTAL_COURSES = 100

object MicrotestPrescriptionMapper {

    fun map(data: PrescriptionHistoryGetResponse): List<HistoricPrescription> {

        val historicPrescriptionOrderPriority = hashMapOf(
                "Requested" to REQUESTED_PRIORITY,
                "Approved" to APPROVED_PRIORITY,
                "Rejected" to REJECTED_PRIORITY)

        val microtestPrescriptionStatusToDisplayedStatus = mapOf(
                PrescriptionStatus.Requested to "Requested",
                PrescriptionStatus.Rejected to "Rejected",
                PrescriptionStatus.Confirmed to "Approved",
                PrescriptionStatus.Cancelled to "Unknown"
        )

        var totalCoursesRunningTotal = 0

        val prescriptionCourses = data.courses.filter { it.type == CourseStatus.Repeat.toString() }

        val historicPrescriptions = ArrayList<HistoricPrescription>()

        for (prescriptionCourse in prescriptionCourses.toList().sortedByDescending { it.orderDate }){

            if (totalCoursesRunningTotal >= UPPER_LIMIT_FOR_TOTAL_COURSES) {
                break
            }

            val datetime = DateTime.parse(prescriptionCourse.orderDate)
                    .toString(DateTimeFormats.frontendDateFormat)

            val historicPrescription = HistoricPrescription(
                    name = prescriptionCourse.name,
                    dosage = pages.prescription
                            .resolveDetailsField(prescriptionCourse.dosage, prescriptionCourse.quantity)
                            .joinToString(" - ")
            )

            historicPrescription.orderDate = datetime
            historicPrescription.status = microtestPrescriptionStatusToDisplayedStatus[prescriptionCourse.status]

            historicPrescriptions.add(historicPrescription)

            totalCoursesRunningTotal++
        }

        val historicPrescriptionsOrderedByStatusOnScreen =
                historicPrescriptions.sortedWith(compareBy({ historicPrescriptionOrderPriority[it.status] }))

        return historicPrescriptionsOrderedByStatusOnScreen
    }
}
