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
import mocking.microtest.myRecord.TestResults
import mocking.microtest.myRecord.TestResultData
import mocking.microtest.myRecord.InrResult
import mocking.microtest.myRecord.InrResults
import mocking.microtest.myRecord.PathResult
import mocking.microtest.myRecord.PathResults
import mocking.microtest.myRecord.MedicalHistory
import mocking.microtest.myRecord.MedicalHistories
import mocking.microtest.myRecord.Recall
import mocking.microtest.myRecord.Recalls
import mocking.microtest.myRecord.TestResultOptions

import utils.set

object MicrotestMyRecordData {

    fun getEmptyMicrotestMyRecord(): MyRecordResponseModel {

        val allergies = Allergies("true", "false", 0, mutableListOf<Allergy>())
        val medications = Medications("true", "false", 0, mutableListOf<Medication>())
        val immunisations = Immunisations("true", "false", 0, mutableListOf<Immunisation>())
        val problems = Problems("true", "false", 0, mutableListOf<Problem>())

        val inrResults = InrResults(0, mutableListOf<InrResult>())
        val pathResults = PathResults(0, mutableListOf<PathResult>())
        val testResultData = TestResultData(inrResults, pathResults)
        val testResults = TestResults("true", "false", 0, testResultData)
        val medicalHistories = MedicalHistories("true", "false",0, mutableListOf<MedicalHistory>())
        val recalls = Recalls("true", "false",0, mutableListOf<Recall>())

        return MyRecordResponseModel(
                allergies, medications, immunisations, problems, testResults, medicalHistories, recalls)
    }

    fun getPopulatedMicrotestMyRecord(myRecordModuleCounts: MyRecordModuleCounts,
                                      testResultOptions: TestResultOptions): MyRecordResponseModel {

        val allergies = buildAllergies(myRecordModuleCounts.allergyCount)
        val medications = buildMedications(myRecordModuleCounts.medicationCount)
        val immunisations = buildImmunisations(myRecordModuleCounts.vaccinationsCount)
        val problems = buildProblems(myRecordModuleCounts.problemCount)
        val testResults = buildTestResults(myRecordModuleCounts, testResultOptions)
        val medicalHistories = buildMedicalHistory(myRecordModuleCounts.medicalHistoryCount)
        val recalls = buildRecalls(myRecordModuleCounts.recallCount)

        val myRecordResponseModel =  MyRecordResponseModel(
                allergies, medications, immunisations, problems, testResults, medicalHistories, recalls)

        MyRecordSerenityHelpers.MY_RECORD_DATA.set(myRecordResponseModel)

        return myRecordResponseModel
    }

    private fun buildAllergies(allergyCount: Int) : Allergies {
        val allergyList = mutableListOf<Allergy>()
        for (i in 1..allergyCount) {
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
        return Allergies("true", "false", allergyList.size, allergyList)
    }

    private fun buildMedications(medicationCount: Int) : Medications {
        val statusList = listOf<String>(MicrotestMedicationStatus.Repeat,
                MicrotestMedicationStatus.Repeat, MicrotestMedicationStatus.Acute)

        val typeList = listOf<String>(MicrotestMedicationType.Current,
                MicrotestMedicationType.History, MicrotestMedicationType.History)

        val medicationList = mutableListOf<Medication>()
        for (i in 1..medicationCount) {
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
        return Medications("true", "false", medicationList.size, medicationList)
    }

    private fun buildImmunisations(vaccinationsCount: Int) : Immunisations {
        val immunisationList = mutableListOf<Immunisation>()
        for (i in 1..vaccinationsCount) {
            immunisationList.add(
                    Immunisation(
                            date = "2019-07-03",
                            description = "Immunisation $i",
                            nextDate = "no next date",
                            status = "Main $i"
                    )
            )
        }
        return Immunisations("true", "false", immunisationList.size, immunisationList)
    }

    private fun buildProblems(problemCount: Int) : Problems {
        val problemList = mutableListOf<Problem>()
        for (i in 1..problemCount) {
            problemList.add(
                    Problem(
                            start_date = "2019-07-03",
                            finish_date = "Ongoing",
                            rubric = "Rubric $i"
                    )
            )
        }
        return Problems("true", "false", problemList.size, problemList)
    }

    private fun buildTestResults(moduleCounts: MyRecordModuleCounts, options: TestResultOptions) : TestResults {
        val inrResultList = buildInrTestResultList(moduleCounts.inrResultCount)
        val pathResultList = buildPathTestResultList(moduleCounts.pathResultCount)
        applyTestResultOptions(options, pathResultList, inrResultList)

        val inrResults = InrResults(inrResultList.size, inrResultList)
        val pathResults = PathResults(pathResultList.size, pathResultList)
        val testResultData = TestResultData(inrResults, pathResults)

        return TestResults("true", "false", 2, testResultData)
    }


    private fun buildInrTestResultList(inrResultCount: Int) : MutableList<InrResult> {
        val inrResultList = mutableListOf<InrResult>()
        for (i in 1..inrResultCount) {
            inrResultList.add(
                    InrResult(recordDateTime = "2019-07-03",
                            codeDescr = "code",
                            therapy = "therapy",
                            target = "target",
                            value = "value",
                            dose = "dose",
                            nextTestDate = "2019-08-03"
                    )
            )
        }
        return inrResultList
    }

    private fun buildPathTestResultList(pathResultCount: Int) : MutableList<PathResult> {
        val pathResultList = mutableListOf<PathResult>()
        for (i in 1..pathResultCount) {
            pathResultList.add(
                    PathResult(
                            name = "ABC",
                            recordDate = "2019-07-03",
                            value = "value",
                            elementName = "element",
                            units = "mg/day",
                            status = "status"
                    )
            )
        }
        return pathResultList
    }

    private fun buildMedicalHistory(medicalHistoryCount: Int) : MedicalHistories {
        val medicalHistoryList = mutableListOf<MedicalHistory>()
        for (i in 1..medicalHistoryCount) {
            medicalHistoryList.add(
                    MedicalHistory(
                            start_date = "2019-07-03",
                            rubric = "Rubric",
                            description = "Description"
                    )
            )
        }
        return MedicalHistories("true", "false", medicalHistoryList.size, medicalHistoryList)
    }

    private fun applyTestResultOptions(
            options: TestResultOptions, pathResults: List<PathResult>, inrResults: List<InrResult>) {

        if (options.includeFilteredOutPathStatuses) {
            val statusList = mutableListOf<String>(
                    MicrotestPathResultStatus.AwaitingResults,
                    "Valid Status",
                    MicrotestPathResultStatus.ResultsReceived)

            for (i in 0..pathResults.size-1) {
                pathResults[i].status = statusList[i]
            }
        }

        if (options.interleavedPathAndInrDates) {
            val inrDateList = mutableListOf<String>(
                    "2019-05-03", "2019-02-03", "2019-03-03")

            val pathDateList = mutableListOf<String>(
                    "2019-04-03", "2019-01-03", "2019-03-06")

            for (i in 0..inrResults.size - 1) {
                inrResults[i].recordDateTime = inrDateList[i % inrDateList.size]
            }

            for (i in 0..pathResults.size - 1) {
                pathResults[i].recordDate = pathDateList[i % pathDateList.size]
            }
        }
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
