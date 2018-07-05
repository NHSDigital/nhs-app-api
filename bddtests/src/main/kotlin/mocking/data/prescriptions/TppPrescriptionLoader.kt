package mocking.data.prescriptions

import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.Medication
import java.util.*

object TppPrescriptionLoader : IPrescriptionLoader<ListRepeatMedicationReply> {
    override lateinit var data:ListRepeatMedicationReply

    override fun loadData(noPrescriptions: Int, noCourses: Int, noRepeats: Int, showDosage: Boolean, showQuantity: Boolean) {
        var medicationList = ListRepeatMedicationReply()
        medicationList.patientId = generateRandomId()
        medicationList.onlineUserId = generateRandomId()
        medicationList.uuid = UUID.randomUUID().toString()

        if(noPrescriptions == 0){
            data = medicationList
            return
        }

        var totalPrescriptions: Int = noPrescriptions
        var totalRequestable: Int = noCourses
        var totalRepeats: Int = noRepeats

        if(noPrescriptions > 100){
            totalPrescriptions = 100
        }
        if(noCourses > 100){
            totalRequestable = 100
        }
        if(noRepeats > 100){
            totalRepeats = 100
        }

        var higherNumber: Int

        higherNumber = if(totalPrescriptions > totalRepeats) {
            totalPrescriptions
        }else{
            totalRepeats
        }

        for(i in 1..higherNumber){
            var medication = Medication()
            medication.drugId = generateRandomId()
            medication.type = "Acute"
            medication.drug = getCourseName()

            if(showDosage || showQuantity) {
                if(showDosage && !showQuantity){
                    medication.details = getDosage()
                }
                if(showQuantity && !showDosage){
                    medication.details = getQuantity()
                }
                if(showQuantity && showDosage){
                    medication.details = getDosage() + " - " + EmisPrescriptionLoader.getQuantity()
                }
            }

            medicationList.Medication.add(medication)
        }

        for(j in 1..totalRequestable){
            medicationList.Medication.elementAt(j-1).requestable = "y"
        }

        for(k in 1..totalRepeats){
            medicationList.Medication.elementAt(k-1).type = "Repeat"
        }

        data = medicationList
    }

    private fun generateRandomId(): String {
        val random = Random()
        val minNum = 100000
        var maxNum = 999999

        return (random.nextInt(maxNum - minNum) + minNum).toString()
    }
}
