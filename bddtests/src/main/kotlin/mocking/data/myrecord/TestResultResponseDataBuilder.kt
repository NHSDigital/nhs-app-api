package mocking.data.myrecord

import mocking.emis.testResults.EffectiveDate
import mocking.emis.testResults.TestResultRange
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultValue
import utils.set

class TestResultResponseDataBuilder {

    fun testResultResponseData(childValueCount: Int = 0, rangePresent: Boolean = true): TestResultResponse{
        val rangeData: TestResultRange?
        val rangeDataList = Pair("3.6","10")
        if(rangePresent) rangeData = TestResultRange(minimumText = rangeDataList.first,
                maximumText = rangeDataList.second) else rangeData = null

        return getTestResultResponse(rangeData,childValueCount)
    }

    private fun getTestResultResponse(rangeData: TestResultRange?,childValueCount : Int = 0) : TestResultResponse{
        val effectiveDate = EffectiveDate("YearMonthDay", "2006-05-15T09:52:44.927")
        val testResultValueData = TestResultValue (
                effectiveDate,
                term = "Basophil count",
                textValue = "5.58",
                numericUnits = "x10^9/L",
                range = rangeData
        )
        MyRecordSerenityHelpers.EXPECTED_TEST_RESULTS_TERM.set(testResultValueData)

        val childValueData = mutableListOf<TestResultValue>()
        if (childValueCount != 0) {
            for(i in 1..childValueCount){
                childValueData.add(testResultValueData)
            }
        }
        return TestResultResponse(testResultValueData,childValueData)
    }
}