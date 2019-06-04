package features.prescriptions.mappers

import constants.DateTimeFormats
import mocking.vision.models.VisionPrescriptionStatus
import mocking.vision.models.PrescriptionHistory
import models.prescriptions.HistoricPrescription
import org.joda.time.DateTime

private const val REQUESTED_PRIORITY = 1
private const val APPROVED_PRIORITY = 2
private const val REJECTED_PRIORITY = 3
private const val UPPER_LIMIT_FOR_TOTAL_COURSES = 100

object VisionPrescriptionMapper {
    fun map(data: PrescriptionHistory) : List<HistoricPrescription> {

        val historicPrescriptionOrderPriority = hashMapOf(
                "Requested" to REQUESTED_PRIORITY,
                "Approved" to APPROVED_PRIORITY,
                "Rejected" to REJECTED_PRIORITY
                )

        val visionPrescriptionStatusToDisplayedStatus = mapOf(
                VisionPrescriptionStatus.InProgress.value to "Requested",
                VisionPrescriptionStatus.Processed.value to "Approved",
                VisionPrescriptionStatus.Rejected.value to "Rejected")

        var totalCoursesRunningTotal = 0

        val historicPrescriptions = ArrayList<HistoricPrescription>()

        for (repeatPrescription in data.request) {
            if (totalCoursesRunningTotal >= UPPER_LIMIT_FOR_TOTAL_COURSES) {
                break
            }

            val prescriptionOrderDate = DateTime.parse(repeatPrescription.date).toString(
                    DateTimeFormats.frontendBasicDateFormat)

            for (course in repeatPrescription.repeat) {
                val historicPrescription = HistoricPrescription(
                        course.drug ?: "",
                        pages.prescription.resolveDetailsField(course.dosage, course.quantity).joinToString(" - ")
                )
                if (repeatPrescription.status != null) {
                    val status = repeatPrescription.status!!
                    historicPrescription.status = visionPrescriptionStatusToDisplayedStatus[status.code.toInt()]
                }
                historicPrescription.orderDate = prescriptionOrderDate
                historicPrescriptions.add(historicPrescription)
                totalCoursesRunningTotal++
            }
        }

        val historicPrescriptionsOrderedByStatusOnScreen =
                historicPrescriptions.sortedWith(compareBy({ historicPrescriptionOrderPriority[it.status] }))


        return historicPrescriptionsOrderedByStatusOnScreen
    }
}
