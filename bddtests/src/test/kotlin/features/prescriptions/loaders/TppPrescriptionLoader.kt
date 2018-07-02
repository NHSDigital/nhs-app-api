package features.prescriptions.loaders

import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.Medication
import java.util.*

object TppPrescriptionLoader {
    fun loadRepeatMedicationData(noPrescriptions: Int, noRequestable: Int, noRepeats: Int, showDosage: Boolean = true, showQuantity: Boolean = true) : ListRepeatMedicationReply {
        var medicationList = ListRepeatMedicationReply()
        medicationList.patientId = generateRandomId()
        medicationList.onlineUserId = generateRandomId()
        medicationList.uuid = UUID.randomUUID().toString()

        if(noPrescriptions == 0){
            return medicationList
        }

        var totalPrescriptions: Int = noPrescriptions
        var totalRequestable: Int = noRequestable
        var totalRepeats: Int = noRepeats

        if(noPrescriptions > 100){
            totalPrescriptions = 100
        }
        if(noRequestable > 100){
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
            medication.drug = EmisPrescriptionLoader.getCourseName()

            if(showDosage || showQuantity) {
                if(showDosage && !showQuantity){
                    medication.details = EmisPrescriptionLoader.getDosage()
                }
                if(showQuantity && !showDosage){
                    medication.details = EmisPrescriptionLoader.getQuantity()
                }
                if(showQuantity && showDosage){
                    medication.details = EmisPrescriptionLoader.getDosage() + " - " + EmisPrescriptionLoader.getQuantity()
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

        return medicationList
    }

    fun generateRandomId(): String {
        val random = Random()
        val minNum = 100000
        var maxNum = 999999

        return (random.nextInt(maxNum - minNum) + minNum).toString()
    }
}
