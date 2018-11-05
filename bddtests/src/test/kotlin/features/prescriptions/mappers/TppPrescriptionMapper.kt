package features.prescriptions.mappers

import mocking.tpp.models.ListRepeatMedicationReply
import models.prescriptions.HistoricPrescription

object TppPrescriptionMapper {
    fun Map(data: ListRepeatMedicationReply) : ArrayList<HistoricPrescription> {

        val historicPrescriptions = ArrayList<HistoricPrescription>()

        for(repeatCourse in data.Medication){
            val historicPrescription = HistoricPrescription(
                    repeatCourse.drug,
                    repeatCourse.details
            )
            historicPrescriptions.add(historicPrescription)
        }


        return historicPrescriptions
    }
}
