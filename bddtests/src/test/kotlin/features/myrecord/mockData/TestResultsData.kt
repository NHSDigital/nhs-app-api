package features.myrecord.mockData

import mocking.emis.testResults.*
import mocking.tpp.models.TestResultsViewReply
import mocking.tpp.models.TestResultsViewReplyItem

object TestResultsData {

    fun getTestResultsForEmis(count: Int): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        for(testResultCount: Int in 1..count) {
            testResults.add(TestResultResponse(value = TestResultValue(
                    effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                    term = "Neutrophil count $testResultCount", textValue = "5.58",
                    numericUnits = "x10^9/L",
                    range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                    childValues = mutableListOf()))
        }

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }

    fun getSingleTestResultWithMultipleChildValuesWithRanges(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                childValues = mutableListOf(
                        TestResultValue(
                                effectiveDate = EffectiveDate("YearMonthDay", "2006-05-15T09:52:44.927"),
                                term = "Platelet count", textValue = "5.9",
                                numericUnits = "x10^9/L", range = TestResultRange(minimumText = "3.6", maximumText = "10")),
                        TestResultValue(
                                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                                term = "Basophil count", textValue = "5.58",
                                numericUnits = "x10^9/L", range = TestResultRange(minimumText = "3.6", maximumText = "10")
                        ))))

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }


    fun getSingleTestResultWithSingleChildValuesWithARange(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                childValues = mutableListOf(
                        TestResultValue(
                                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                                term = "Basophil count", textValue = "5.58",
                                numericUnits = "x10^9/L", range = TestResultRange(minimumText = "3.6", maximumText = "10")
                        ))))

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }

    fun getSingleTestResultWithMultipleChildValuesWithNoRanges(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                childValues = mutableListOf(
                        TestResultValue(
                                effectiveDate = EffectiveDate("YearMonthDay", "2006-05-15T09:52:44.927"),
                                term = "Platelet count", textValue = "5.9",
                                numericUnits = "x10^9/L", range = null),
                        TestResultValue(
                                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                                term = "Basophil count", textValue = "5.58",
                                numericUnits = "x10^9/L", range = null
                        ))))

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }

    fun getSingleTestResultWithSingleChildValuesWithNoRanges(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                childValues = mutableListOf(
                        TestResultValue(
                                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                                term = "Basophil count", textValue = "5.58",
                                numericUnits = "x10^9/L", range = null
                        ))))

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }

    fun getSingleTestResultWithNoChildValuesOrRange(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = null),
                childValues = mutableListOf()))

        return TestResultResponseModel(
                medicalRecord = TestResultMedicalRecord(
                        testResults = testResults
                )
        )
    }

    fun getSingleTestResultWithNoChildValuesAndARange(): TestResultResponseModel {

        val testResults = mutableListOf<TestResultResponse>()

        testResults.add(TestResultResponse(value = TestResultValue(
                effectiveDate = EffectiveDate("Unknown", "2018-05-15T09:52:44.927"),
                term = "Neutrophil count", textValue = "5.58",
                numericUnits = "x10^9/L",
                range = TestResultRange(minimumText = "1.7", maximumText = "6")),
                childValues = mutableListOf()))

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
            testResults.add(TestResultsViewReplyItem(value = "Anticoag Control (Warfarin), Read $testCount", description = "Pathology $testCount",
                    date = "2001-06-28T00:00:00.0Z"))
        }

        return TestResultsViewReply(items = testResults)
    }

    fun getDefaultTppTestResultsData(): TestResultsViewReply {
        return TestResultsViewReply(items = mutableListOf())
    }
}
