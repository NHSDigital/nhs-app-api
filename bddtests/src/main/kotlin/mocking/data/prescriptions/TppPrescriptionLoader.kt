package mocking.data.prescriptions

import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.Medication
import models.prescriptions.PrescriptionLoaderConfiguration
import java.util.*

object TppPrescriptionLoader : IPrescriptionLoader<ListRepeatMedicationReply> {
    override lateinit var data: ListRepeatMedicationReply

    private const val MIN_RANDOM_ID_VALUE = 100000
    private const val MAX_RANDOM_ID_VALUE = 999999
    private const val ITERATIONS_NUMBER = 100

    override fun loadData(prescriptionLoaderConfig: PrescriptionLoaderConfiguration,
                          prescriptionCompletedByProxy: Boolean) {

        val medicationList = ListRepeatMedicationReply()
        medicationList.patientId = generateRandomId()
        medicationList.onlineUserId = generateRandomId()
        medicationList.uuid = UUID.randomUUID().toString()

        if (prescriptionLoaderConfig.noPrescriptions == 0) {
            data = medicationList
            return
        }

        val totalPrescriptions = minOf(prescriptionLoaderConfig.noPrescriptions, ITERATIONS_NUMBER)
        val totalRequestable = minOf(prescriptionLoaderConfig.noCourses, ITERATIONS_NUMBER)
        val totalRepeats = minOf(prescriptionLoaderConfig.noRepeats, ITERATIONS_NUMBER)

        val higherNumber: Int

        higherNumber = if (totalPrescriptions > totalRepeats) {
            totalPrescriptions
        } else {
            totalRepeats
        }

        for (counter in 1..higherNumber) {
            val medication = Medication()
            medication.drugId = generateRandomId()
            medication.type = "Acute"
            medication.drug = getCourseName()
            medication.details = getMedicationDetails(prescriptionLoaderConfig.showQuantity,
                    prescriptionLoaderConfig.showDosage, counter)

            medicationList.Medication.add(medication)
        }

        for (j in 1..totalRequestable) {
            medicationList.Medication.elementAt(j - 1).requestable = "y"
        }

        for (k in 1..totalRepeats) {
            medicationList.Medication.elementAt(k - 1).type = "Repeat"
        }

        data = medicationList
    }

    private fun generateRandomId(): String {
        val random = Random()
        val minNum = MIN_RANDOM_ID_VALUE
        val maxNum = MAX_RANDOM_ID_VALUE

        return (random.nextInt(maxNum - minNum) + minNum).toString()
    }

    private fun getMedicationDetails(showQuantity: Boolean, showDosage: Boolean, value: Int): String {
        val medicationDetails = arrayListOf<String>()
        if (showDosage) {
            medicationDetails.add(getDosage())
        }
        if (showQuantity) {
            medicationDetails.add(getQuantity(value))
        }
        return medicationDetails.joinToString(separator = "-")
    }
}
