package mocking.tpp.data

import mocking.tpp.models.TestResultsViewReply
import mocking.tpp.models.TestResultsViewReplyItem
import java.time.OffsetDateTime

const val TEST_RESULT_ID = "TestResultId"
class TestResultsDataTpp {

    private val description = "A descriptive description"
    private val date = OffsetDateTime.now().minusDays(MINUS_DAYS).toString()
    private val value = "A value with values"
    private val detailedValue = "A more detailed value which is perfect for the values section"

    var testResultData = TestResultsViewReply(items =
    arrayListOf(TestResultsViewReplyItem(TEST_RESULT_ID, description, date, value)))
    var detailedTestResultData = TestResultsViewReply(items = arrayListOf(TestResultsViewReplyItem(TEST_RESULT_ID,
            description, date, detailedValue)))
}
