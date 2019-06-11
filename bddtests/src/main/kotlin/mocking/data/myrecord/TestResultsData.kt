package mocking.data.myrecord

import mocking.emis.testResults.TestResultMedicalRecord
import mocking.emis.testResults.TestResultResponse
import mocking.emis.testResults.TestResultResponseModel
import mocking.tpp.models.TestResultsViewReply
import mocking.tpp.models.TestResultsViewReplyItem
import java.io.File
import java.nio.file.Paths

const val NUMBER_OF_TEST_RESULT_RECORDS = 2

class TestResultsData {

    companion object{
        const val mockTestResultId: String = "C435000000000000"

        fun getTestResultsForEmis(count: Int): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            for(testResultCount: Int in 1..count) {
                testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                        childValueCount = 0))
            }
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
        }

        fun getSingleTestResultWithMultipleChildValuesWithRanges(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 2
            ))
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    ))
        }

        fun getSingleTestResultWithSingleChildValuesWithARange(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 1
            ))
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    ))
        }

        fun getSingleTestResultWithMultipleChildValuesWithNoRanges(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()

            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 2,
                    rangePresent = false
            ))

            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
        }

        fun getSingleTestResultWithSingleChildValuesWithNoRanges(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()

            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 1,
                    rangePresent = false
            ))

            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
        }

        fun getSingleTestResultWithNoChildValuesOrRange(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 0,
                    rangePresent = false
            ))
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
        }

        fun getSingleTestResultWithNoChildValuesAndARange(): TestResultResponseModel {
            val testResults = mutableListOf<TestResultResponse>()
            testResults.add(TestResultResponseDataBuilder().testResultResponseData(
                    childValueCount = 0
            ))
            return TestResultResponseModel(
                    medicalRecord = TestResultMedicalRecord(
                            testResults = testResults
                    )
            )
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

        fun getVisionTestResultsDataWithNoTestResults(): String {
            val response = "<![CDATA[<root><patient>"
            val responseStringEnd = "</patient></root>]]>"

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

