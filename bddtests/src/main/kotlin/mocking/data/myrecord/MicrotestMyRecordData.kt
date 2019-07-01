package mocking.data.myrecord

import mocking.microtest.myRecord.Medication
import mocking.microtest.myRecord.Medications
import mocking.microtest.myRecord.MyRecordResponseModel
import mocking.microtest.myRecord.Allergy
import mocking.microtest.myRecord.Allergies

import utils.set

object MicrotestMyRecordData {

    const val MAX_ALLERGIES = 3
    const val MAX_MEDICATIONS = 3

    fun getEmptyMicrotestMyRecord(): MyRecordResponseModel {

        val allergies = Allergies("true", "false", 0, mutableListOf<Allergy>())
        val medications = Medications("true", "false", 0, mutableListOf<Medication>())
        return MyRecordResponseModel(allergies, medications)
    }

    fun getPopulatedMicrotestMyRecord(): MyRecordResponseModel {

        val allergyList = mutableListOf<Allergy>()
        for (i in 1..MAX_ALLERGIES) {
            allergyList.add(
                    Allergy(
                            id = "$i",
                            type = "Drug",
                            start_date = "2019-03-27",
                            description = "Penicilin $i",
                            severity = "Low"
                    )
            )
        }

        val statusList = listOf<String>(MicrotestMedicationStatus.Repeat,
                MicrotestMedicationStatus.Repeat, MicrotestMedicationStatus.Acute)

        val typeList = listOf<String>(MicrotestMedicationType.Current,
                MicrotestMedicationType.History, MicrotestMedicationType.History)

        val medicationList = mutableListOf<Medication>()
        for (i in 1..MAX_MEDICATIONS) {
            medicationList.add(
                    Medication(
                            id = "$i",
                            name = "Medication $i",
                            quantity = "60 tabs",
                            dosage = "ONE tablet every day",
                            status = statusList[i-1],
                            type = typeList[i-1],
                            prescribed_date = "2019-03-27",
                            first_prescribed_date = "2019-03-27",
                            reason = "Reason: high blood pressure"
                    )
            )
        }

        val allergies = Allergies("true", "false", allergyList.size, allergyList)
        val medications = Medications("true", "false", medicationList.size, medicationList)
        val  myRecordResponseModel =  MyRecordResponseModel(allergies, medications)

        MyRecordSerenityHelpers.MY_RECORD_DATA.set(myRecordResponseModel)

        return myRecordResponseModel
    }

}
