package features.myrecord.factories

import mocking.data.myrecord.MicrotestPathResultStatus
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.InrResult
import mocking.microtest.myRecord.MyRecordResponseModel
import mocking.microtest.myRecord.PathResult
import models.Patient
import utils.getOrFail
import worker.models.myrecord.TestResultItem


class TestResultsFactoryMicrotest : TestResultsFactory() {

    override fun respondWithACorruptedResponse(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun getExpectedTestResults(): List<TestResultItem> {

        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val inrResults = myRecord.testResults.data.inrResults.data
        val pathResults = myRecord.testResults.data.pathResults.data

        val inrResultItems = mutableListOf<TestResultItem>()
        val pathResultItems = mutableListOf<TestResultItem>()
        val testResultItems = mutableListOf<TestResultItem>()

        for (inrResult in inrResults) {
            inrResultItems.add(buildTestResultItem(inrResult))
        }
        val sortedInrResults = inrResultItems.sortedByDescending{ it.date }

        for (pathResult in pathResults) {
            if (pathResult.status != MicrotestPathResultStatus.AwaitingResults) {
                pathResultItems.add(buildTestResultItem(pathResult))
            }
        }
        val sortedPathResults = pathResultItems.sortedByDescending{ it.date }

        testResultItems.addAll(sortedInrResults)
        testResultItems.addAll(sortedPathResults)

        return testResultItems
    }

    private fun buildTestResultItem(inrResult: InrResult) : TestResultItem {
        return TestResultItem(
            date = worker.models.myrecord.Date(
                value = inrResult.recordDateTime,
                datePart = "YearMonthDay"),
            description = "",
            testResultChildLineItems = mutableListOf(),
            associatedTexts = mutableListOf(
                    "INR Results: " + inrResult.value + " (target - " + inrResult.target + ")",
                    "Condition: " + inrResult.codeDescr,
                    "Therapy: " + inrResult.therapy,
                    "Dose: " + inrResult.dose,
                    "Next test date: " + inrResult.nextTestDate            )
        )
    }

    private fun buildTestResultItem(pathResult: PathResult) : TestResultItem {
        return TestResultItem(
                date = worker.models.myrecord.Date(
                        value = pathResult.recordDate,
                        datePart = "Unknown"),
                description = "",
                testResultChildLineItems = mutableListOf(),
                associatedTexts = mutableListOf(
                        pathResult.name + ": " + pathResult.elementName,
                        "Value: " + pathResult.value,
                        "Units: " + pathResult.units
                )
        )
    }


    override fun disabled(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithBlankRecord(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun enabledWithRecords(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun errorRetrieving(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }

    override fun noAccess(patient: Patient) {
        throw UnsupportedOperationException("Not yet implemented")
    }
}
