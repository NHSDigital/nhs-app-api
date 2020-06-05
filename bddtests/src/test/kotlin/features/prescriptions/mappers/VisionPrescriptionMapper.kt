package features.prescriptions.mappers

import constants.DateTimeFormats
import mocking.vision.models.VisionPrescriptionStatus
import mocking.vision.models.PrescriptionHistory
import models.prescriptions.HistoricPrescription
import org.joda.time.DateTime

private const val UPPER_LIMIT_FOR_TOTAL_COURSES = 100

object VisionPrescriptionMapper {
    fun map(data: PrescriptionHistory) : List<HistoricPrescription> {

        val visionPrescriptionStatusToDisplayedStatus = mapOf(
                VisionPrescriptionStatus.InProgress.value to "Requested",
                VisionPrescriptionStatus.Processed.value to "Approved",
                VisionPrescriptionStatus.Rejected.value to "Rejected")

        var totalCoursesRunningTotal = 0

        val historicPrescriptions = ArrayList<HistoricPrescription>()
        val sortedList = data.request.toList().sortedWith(compareBy{ it })

        for (repeatPrescription in sortedList) {
            if (totalCoursesRunningTotal >= UPPER_LIMIT_FOR_TOTAL_COURSES) {
                break
            }

            val prescriptionOrderDate = DateTime.parse(repeatPrescription.date).toString(
                    DateTimeFormats.frontendDateFormat)

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

        return historicPrescriptions
    }
}
