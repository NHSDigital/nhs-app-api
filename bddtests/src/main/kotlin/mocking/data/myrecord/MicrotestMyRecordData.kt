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
import utils.set

object MicrotestMyRecordData {

    fun getEmptyMicrotestMyRecord(): MyRecordResponseModel {

        val allergies = Allergies("true", "false", 0, mutableListOf<Allergy>())
        val medications = Medications("true", "false", 0, mutableListOf<Medication>())
        val immunisations = Immunisations("true", "false", 0, mutableListOf<Immunisation>())
        val problems = Problems("true", "false", 0, mutableListOf<Problem>())
        return MyRecordResponseModel(allergies, medications, immunisations, problems)
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
        val myRecordResponseModel =  MyRecordResponseModel(allergies, medications, immunisations, problems)

        MyRecordSerenityHelpers.MY_RECORD_DATA.set(myRecordResponseModel)

        return myRecordResponseModel
    }

}
