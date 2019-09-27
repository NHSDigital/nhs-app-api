package mocking.data.myrecord

import mocking.microtest.myRecord.Medication
import mocking.microtest.myRecord.Medications
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.MyRecordResponseModel
import mocking.microtest.myRecord.Allergy
import mocking.microtest.myRecord.Allergies
import mocking.microtest.myRecord.Immunisation
import mocking.microtest.myRecord.Immunisations
import mocking.microtest.myRecord.Problem
import mocking.microtest.myRecord.Problems
import mocking.microtest.myRecord.MedicalHistory
import mocking.microtest.myRecord.MedicalHistories
import mocking.microtest.myRecord.Recall
import mocking.microtest.myRecord.Recalls
import utils.set

object MicrotestMyRecordData {

    fun getEmptyMicrotestMyRecord(): MyRecordResponseModel {

        val allergies = Allergies("true", "false", 0, mutableListOf<Allergy>())
        val medications = Medications("true", "false", 0, mutableListOf<Medication>())
        val immunisations = Immunisations("true", "false", 0, mutableListOf<Immunisation>())
        val problems = Problems("true", "false", 0, mutableListOf<Problem>())
        val medicalHistories = MedicalHistories("true", "false",0, mutableListOf<MedicalHistory>())
        val recalls = Recalls("true", "false",0, mutableListOf<Recall>())
        return MyRecordResponseModel(allergies, medications, immunisations, problems, medicalHistories, recalls)
    }

    fun getPopulatedMicrotestMyRecord(myRecordModuleCounts: MyRecordModuleCounts): MyRecordResponseModel {

        val allergyList = mutableListOf<Allergy>()
        for (i in 1..myRecordModuleCounts.allergyCount) {
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
        for (i in 1..myRecordModuleCounts.medicationCount) {
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

        val immunisationList = mutableListOf<Immunisation>()
        for (i in 1..myRecordModuleCounts.vaccinationsCount) {
            immunisationList.add(
                    Immunisation(
                            date = "2019-07-03",
                            description = "Immunisation $i",
                            nextDate = "no next date",
                            status = "Main $i"
                    )
            )
        }

        val problemList = mutableListOf<Problem>()
        for (i in 1..myRecordModuleCounts.problemCount) {
            problemList.add(
                    Problem(
                            start_date = "2019-07-03",
                            finish_date = "Ongoing",
                            rubric = "Rubric $i"
                    )
            )
        }

        val allergies = Allergies("true", "false", allergyList.size, allergyList)
        val medications = Medications("true", "false", medicationList.size, medicationList)
        val immunisations = Immunisations("true", "false", immunisationList.size, immunisationList)
        val problems = Problems("true", "false", problemList.size, problemList)
        val medicalHistories = buildMedicalHistories(myRecordModuleCounts.medicalHistoryCount)
        val recalls = buildRecalls(myRecordModuleCounts.recallCount)

        val myRecordResponseModel =  MyRecordResponseModel(
                allergies, medications, immunisations, problems, medicalHistories, recalls)

        MyRecordSerenityHelpers.MY_RECORD_DATA.set(myRecordResponseModel)

        return myRecordResponseModel
    }

    private fun buildMedicalHistories(medicalHistoryCount: Int) : MedicalHistories {
        val medicalHistoryList = mutableListOf<MedicalHistory>()
        for (i in 1..medicalHistoryCount) {
            medicalHistoryList.add(
                    MedicalHistory(
                            start_date = "2019-07-03",
                            rubric = "Rubric $i",
                            description = "Description $i"
                    )
            )
        }
        return MedicalHistories("true", "false", medicalHistoryList.size,  medicalHistoryList)
    }

    private fun buildRecalls(recallsCount: Int) : Recalls {

        val recallList = mutableListOf<Recall>()
        for (i in 1..recallsCount) {

            var recordDate = "2019-0$i-01"
            if (i == 1) {
                recordDate = ""
            }

            recallList.add(
                    Recall(
                            recordDate = recordDate,
                            name = "Name $i",
                            description = "Desc $i",
                            result = "Result $i",
                            nextDate = "NextDate $i",
                            status = "Status $i"
                    )
            )
        }

        return Recalls("true", "false", recallList.size, recallList)
    }

}
