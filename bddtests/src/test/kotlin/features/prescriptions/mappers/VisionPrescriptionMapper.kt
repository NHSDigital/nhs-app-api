package features.prescriptions.mappers

import constants.DateTimeFormats
import mocking.emis.models.VisionPrescriptionStatus
import mocking.vision.models.PrescriptionHistory
import models.prescriptions.HistoricPrescription
import org.joda.time.DateTime

object VisionPrescriptionMapper {
    fun map(data: PrescriptionHistory) : List<HistoricPrescription> {

        val historicPrescriptionOrderPriority = hashMapOf( "Rejected" to 1, "Requested" to 2, "Approved" to 3)

        val visionPrescriptionStatusToDisplayedStatus = mapOf(
                VisionPrescriptionStatus.InProgress.value to "Requested",
                VisionPrescriptionStatus.Processed.value to "Approved",
                VisionPrescriptionStatus.Rejected.value to "Rejected")

        var totalCoursesRunningTotal = 0

        val historicPrescriptions = ArrayList<HistoricPrescription>()

        for (repeatPrescription in data.request) {
            if (totalCoursesRunningTotal >= 100) {
                break
            }

            val prescriptionOrderDate = DateTime.parse(repeatPrescription.date).toString(
                    DateTimeFormats.frontendBasicDateFormat)

            for (course in repeatPrescription.repeat) {
                val historicPrescription = HistoricPrescription(
                        course.drug ?: "",
                        pages.prescription.resolveDetailsField(course.dosage, course.quantity)
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
