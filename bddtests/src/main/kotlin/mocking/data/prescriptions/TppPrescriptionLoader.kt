package mocking.data.prescriptions

import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.Medication
import java.util.*

object TppPrescriptionLoader : IPrescriptionLoader<ListRepeatMedicationReply> {
    override lateinit var data:ListRepeatMedicationReply

    private const val MIN_RANDOM_ID_VALUE = 100000
    private const val MAX_RANDOM_ID_VALUE = 999999
    private const val ITERATIONS_NUMBER = 100

    @Suppress("ComplexMethod")
    override fun loadData(noPrescriptions: Int, noCourses: Int,
                          noRepeats: Int, showDosage: Boolean,
                          showQuantity: Boolean) {
        var medicationList = ListRepeatMedicationReply()
        medicationList.patientId = generateRandomId()
        medicationList.onlineUserId = generateRandomId()
        medicationList.uuid = UUID.randomUUID().toString()

        if(noPrescriptions == 0) {
            data = medicationList
            return
        }

        var totalPrescriptions: Int = noPrescriptions
        var totalRequestable: Int = noCourses
        var totalRepeats: Int = noRepeats

        if(noPrescriptions > ITERATIONS_NUMBER) {
            totalPrescriptions = ITERATIONS_NUMBER
        }

        if(noCourses > ITERATIONS_NUMBER) {
            totalRequestable = ITERATIONS_NUMBER
        }

        if(noRepeats > ITERATIONS_NUMBER) {
            totalRepeats = ITERATIONS_NUMBER
        }

        var higherNumber: Int

        higherNumber = if(totalPrescriptions > totalRepeats) {
            totalPrescriptions
        } else {
            totalRepeats
        }

        for(i in 1..higherNumber) {
            var medication = Medication()
            medication.drugId = generateRandomId()
            medication.type = "Acute"
            medication.drug = getCourseName()

            if(showDosage || showQuantity) {
                if(showDosage && !showQuantity) {
                    medication.details = getDosage()
                }

                if(showQuantity && !showDosage) {
                    medication.details = getQuantity()
                }

                if(showQuantity && showDosage) {
                    medication.details = getDosage() + " - " + EmisPrescriptionLoader.getQuantity()
                }
            }

            medicationList.Medication.add(medication)
        }

        for(j in 1..totalRequestable) {
            medicationList.Medication.elementAt(j-1).requestable = "y"
        }

        for(k in 1..totalRepeats) {
            medicationList.Medication.elementAt(k-1).type = "Repeat"
        }

        data = medicationList
    }

    private fun generateRandomId(): String {
        val random = Random()
        val minNum = MIN_RANDOM_ID_VALUE
        var maxNum = MAX_RANDOM_ID_VALUE

        return (random.nextInt(maxNum - minNum) + minNum).toString()
    }
}
