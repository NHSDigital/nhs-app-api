package mocking.data.myrecord


import mocking.emis.testResults.AssociatedText
import mocking.emis.testResults.EffectiveDate
import mocking.emis.testResults.TestResultRange
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultValue
import org.joda.time.DateTime
import utils.set

class TestResultResponseDataBuilder {

    fun testResultResponseData(childValueCount: Int = 0, rangePresent: Boolean = true,
                               date: DateTime): TestResultResponse {
        val rangeData: TestResultRange?
        val rangeDataList = Pair("3.6", "10")
        if(rangePresent) rangeData = TestResultRange(minimumText = rangeDataList.first,
                maximumText = rangeDataList.second) else rangeData = null

        return getTestResultResponse(rangeData, childValueCount, date)
    }

    private fun getTestResultResponse(rangeData: TestResultRange?, childValueCount : Int = 0,
                                      date: DateTime) : TestResultResponse{
        val effectiveDate = EffectiveDate("YearMonthDay", date.toString())
        val associatedText = mutableListOf(AssociatedText("Test result comment", "PO"))
        val testResultValueData: TestResultValue
        val childTestResults = mutableListOf<TestResultValue>()

        if (childValueCount != 0) {

            testResultValueData = TestResultValue(
                    effectiveDate,
                    term = "Test result term",
                    associatedText = associatedText
            )

            val childTestResultValueData = TestResultValue(
                    effectiveDate,
                    term = "Test result component term",
                    textValue = "5.58",
                    numericUnits = "x10^9/L",
                    range = rangeData
            )
            MyRecordSerenityHelpers.EXPECTED_TEST_RESULT_CHILD.set(childTestResultValueData)

            (1..childValueCount).forEach { _ ->
                childTestResults.add(childTestResultValueData)
            }
        } else {
            testResultValueData = TestResultValue(
                    effectiveDate,
                    term = "Test result term",
                    associatedText = associatedText,
                    textValue = "5.58",
                    numericUnits = "x10^9/L",
                    range = rangeData
            )
        }
        MyRecordSerenityHelpers.EXPECTED_TEST_RESULT.set(testResultValueData)
        return TestResultResponse(testResultValueData,childTestResults)
    }
}
