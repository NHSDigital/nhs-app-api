package mocking.data.myrecord

import mocking.emis.testResults.TestResultMedicalRecord
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultResponseModel
import mocking.tpp.models.TestResultsViewReply
import mocking.tpp.models.TestResultsViewReplyItem
import org.joda.time.DateTime
import java.io.File
import java.nio.file.Paths

const val NUMBER_OF_TEST_RESULT_RECORDS = 3

class TestResultsData {

    companion object{
        const val mockTestResultId: String = "C435000000000000"
        private const val DATE_FOR_TEST_RESULT_YEAR = 2006
        private const val DATE_FOR_TEST_RESULT_MONTH = 5
        private const val DATE_FOR_TEST_RESULT_DAY = 15

        fun getTestResultsForEmis(count: Int): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()

            val date = DateTime().withDate(DATE_FOR_TEST_RESULT_YEAR, DATE_FOR_TEST_RESULT_MONTH,
                    DATE_FOR_TEST_RESULT_DAY-1)
            for(testResultCount: Int in 1..count) {
                testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                        childValueCount = 0,
                        date = date.plusDays(testResultCount)))
            }
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
        }

        fun getTestResultWithChildValueCountAndRangePresent(
                childValueCount: Int,
                rangePresent: Boolean = true
        ): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = childValueCount,
                    rangePresent = rangePresent,
                    date = DateTime().withDate(DATE_FOR_TEST_RESULT_YEAR, DATE_FOR_TEST_RESULT_MONTH,
                            DATE_FOR_TEST_RESULT_DAY)
            ))

            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    ))
        }

        fun getThreeTestResultsWhereTheSecondRecordHasNoDate(): TestResultResponseModel {
            val testResults = getTestResultsForEmis(NUMBER_OF_TEST_RESULT_RECORDS)
            testResults.medicalRecord.testResults[1].value.effectiveDate = null

            return testResults
        }

        fun getDefaultTestResultsModel(): TestResultResponseModel {
            return TestResultResponseModel(
                    medicalRecord =  TestResultMedicalRecord(
                            testResults = mutableListOf()
                    ))
        }

        fun getMultipleTestResultsForTpp(count: Int): TestResultsViewReply {
            val testResults = mutableListOf<TestResultsViewReplyItem>()

            for (testCount: Int in 1..count) {
                testResults.add(TestResultsViewReplyItem(id = mockTestResultId,
                        value = "Anticoag Control (Warfarin), Read $testCount",
                        description = "Pathology $testCount",
                        date = "2001-06-28T00:00:00.0Z"))
            }

            return TestResultsViewReply(items = testResults)
        }

        fun getDefaultTppTestResultsData(): TestResultsViewReply {
            return TestResultsViewReply(items = mutableListOf())
        }

        fun getTestResultDetail(): TestResultsViewReply {
            val testResultDetail = TestResultsViewReply(items = mutableListOf())

            testResultDetail.items.add(TestResultsViewReplyItem(value = "<p>Test Result Detail</p>"))

            return testResultDetail
        }

        fun getTestResultDetailWithHTMLEntities(): TestResultsViewReply {
            val testResultDetail = TestResultsViewReply(items = mutableListOf())

            val path = Paths.get("").toAbsolutePath().toString()
            val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/TPPTestResultWithHTMLEntities.html"

            testResultDetail.items.add(TestResultsViewReplyItem(
                    value = File(fileLocation).readText()))

            return testResultDetail
        }

        fun getVisionTestResultsDataWithNoTestResults(isBadData: Boolean = false): String {

            var response = "<![CDATA[<root><patient>"
            val responseStringEnd = "</patient></root>]]>"

            if (isBadData) {
                response = "<![BADDATA[<root><patient>"
            }

            return response + responseStringEnd
        }


        fun getVisionTestResultsDataWithMultipleResults(): String {

            val response = "<![CDATA[<root><patient>"
            val responseStringEnd = "</patient></root>]]>"

            val path = Paths.get("").toAbsolutePath().toString()
            val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/VariousTestResults.html"
            val html = File(fileLocation).readText()

            return response + html + responseStringEnd
        }
    }

}

