package features.myrecord.factories

import mocking.data.myrecord.MicrotestMedicationStatus
import mocking.data.myrecord.MicrotestMedicationType
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.Medication
import mocking.microtest.myRecord.MyRecordResponseModel
import models.Patient
import utils.getOrFail
import worker.models.myrecord.MedicationsData
import worker.models.myrecord.MedicationItem
import worker.models.myrecord.MedicationLineItem

class MedicationsFactoryMicrotest: MedicationsFactory(){

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException()
    }

    override fun getExpectedMedications(): MedicationsData {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val medications = myRecord.drugs.data

        val acuteMeds = mutableListOf<MedicationItem>()
        val currentMeds = mutableListOf<MedicationItem>()
        val historicMeds = mutableListOf<MedicationItem>()

        for(item in medications) {
            if (item.status.equals(MicrotestMedicationStatus.Repeat)
                    && item.type.equals(MicrotestMedicationType.Current)) {
                currentMeds.add(buildMedicationItem(item))
            } else if (item.status.equals(MicrotestMedicationStatus.Repeat)
                    && item.type.equals(MicrotestMedicationType.History)) {
                historicMeds.add(buildMedicationItem(item))
            } else if (item.status.equals(MicrotestMedicationStatus.Acute)) {
                acuteMeds.add(buildMedicationItem(item))
            }
        }

        return MedicationsData(acuteMeds, currentMeds, historicMeds)
    }

    private fun buildMedicationItem(med : Medication): MedicationItem {
        return MedicationItem(
                date = med.prescribed_date,
                lineItems = mutableListOf(
                        MedicationLineItem(
                                text = med.name,
                                lineItems = mutableListOf<String>()
                        ),
                        MedicationLineItem(
                                text = med.dosage,
                                lineItems = mutableListOf<String>()
                        ),
                        MedicationLineItem(
                                text = med.quantity,
                                lineItems = mutableListOf<String>()
                        ),
                        MedicationLineItem(
                                text = "Reason: " + med.reason,
                                lineItems = mutableListOf<String>()
                        )
                )
        )
    }
}