package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.TestResultsFactory
import mocking.MockingClient
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.data.myrecord.TestResultsData
import mocking.emis.testResults.TestResultValue
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ZERO = 0
private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ONE = 1
private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO = 2
private const val NUMBER_OF_TEST_RESULTS_EQUALS_FOUR = 4

open class MedicalRecordTestResultsStepDefinitionsBackend {

    private val mockingClient = MockingClient.instance

    @Given("^an error occurs retrieving the test result detail$")
    fun givenAnErrorOccursGettingTestResultDetailForTpp() {
        mockingClient.forTpp.mock {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithServiceNotAvailableException()
        }
    }

    @Given("^the test result details are retrieved successfully$")
    fun successGettingTestResultDetailForTpp() {
        mockingClient.forTpp.mock {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_FOUR))
        }
    }

    @Given("^the GP Practice has test result details$")
    fun givenTestResultDetailIsRetrievedSuccessfully() {
        mockingClient.forTpp.mock {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetail())
        }
    }

    @Given("^the GP Practice has test result details with HTML entities$")
    fun givenTestResultDetailWithHTMLEntitiesIsRetrievedSuccessfully() {
        mockingClient.forTpp.mock {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetailWithHTMLEntities())
        }
    }

    @Given("^the GP Practice has six test results$")
    fun givenTheGpPracticeHasSixTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has a single test result with multiple child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesForEmis() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(
                                    NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO, false))
        }
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesForEmis() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(
                                    NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ONE, false))
        }
    }

    @Given("^the GP Practice sends a bad test results response$")
    fun givenTheGpPracticeHasSendCorruptedContent() {
        TestResultsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .respondWithACorruptedResponse(SerenityHelpers.getPatient())
    }

    @Given("^the EMIS GP Practice has three test results where the second record has no date$")
    fun givenTheEmisGpPracticeHasATestResultWithNoDate() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getThreeTestResultsWhereTheSecondRecordHasNoDate())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for EMIS$")
    fun givenTheGpPracticeHasTwoTestResultsWithMultipleChildValuesWithRangesFor() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO))
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ONE))
        }
    }

    @Given("^the GP Practice has test results enabled " +
            "and a single test result exists with no child values or range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(
                                    NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ZERO, false))
        }
    }

    @Given("^the GP Practice has a single test result with no child values and range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor() {
        mockingClient.forEmis.mock {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ZERO))
        }
    }

    @Given("^I do not have access to test results$")
    fun givenIDoNotHaveAccessToTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^I have no test results$")
    fun givenIHaveNoTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^an error occurs retrieving the test results$")
    fun givenAnErrorOccurrsRetrievingTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled test results functionality$")
    fun butTheGPPracticeHasDisabledTestResultsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @When("^I get the users test results$")
    fun whenIGetTheUsersMyRecordData() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .myRecord.getMyRecord(patientId)

        Serenity.setSessionVariable(MyRecordResponse::class).to(result)
    }

    @Then("^the flag informing that the patient has access to the test results data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.testResults.hasAccess)
    }

    @When("^the flag informing that there was an error retrieving the test results data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.testResults.hasErrored)
    }

    @Then("^I receive (.*) test results as part of the my record object$")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.testResults.data.count())
    }

    @Then("^I receive the test result with term set correctly to Term$")
    fun thenIReceiveATestResultWithTermSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        val testResultValue =
                MyRecordSerenityHelpers.EXPECTED_TEST_RESULT.getOrFail<TestResultValue>()
        assertEquals(testResultValue.term, result.response.testResults.data.first().description)
    }

    @Then("^the line item displays text value and range$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectlyIncludingRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        val testResultValue =
                MyRecordSerenityHelpers.EXPECTED_TEST_RESULT_CHILD.getOrFail<TestResultValue>()
        val lowerRange = testResultValue.range!!.minimumText
        val upperRange = testResultValue.range!!.maximumText
        assertEquals("Child LineItem Description does not match",
                "${testResultValue.term}: ${testResultValue.textValue} " +
                        "${testResultValue.numericUnits} (normal range: $lowerRange - $upperRange)",
                result.response.testResults.data.first().testResultChildLineItems.first().description)
    }

    @Then("^the line item value is set correctly$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        val testResultValue =
                MyRecordSerenityHelpers.EXPECTED_TEST_RESULT_CHILD.getOrFail<TestResultValue>()
        assertEquals("Child LineItem Description does not match",
                "${testResultValue.term}: ${testResultValue.textValue} " +
                        "${testResultValue.numericUnits}",
                result.response.testResults.data.first().testResultChildLineItems.first().description)
    }

    @Then("^I receive line items for each child value$")
    fun thenIReceiveATestResultWithLineItemsForEachChildValue() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Expected two ChildLineItems in TestResult",
                2,
                result.response.testResults.data.first().testResultChildLineItems.count())
    }

    @Then("^the field indicating supplier is set$")
    fun thenTheFlagIndicatingSupplierIsSetTo() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(SerenityHelpers.getGpSupplier().toString().toUpperCase(), result.response.supplier.toUpperCase())
    }

    @Then("^I receive a single test result with the term set correctly to Term TextValue NumericUnits$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnits() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        val testResultValue =
                MyRecordSerenityHelpers.EXPECTED_TEST_RESULT.getOrFail<TestResultValue>()
        assertEquals("${testResultValue.term}: ${testResultValue.textValue} " +
                "${testResultValue.numericUnits}", result.response.testResults.data.first().description)
    }

    @Then("^I receive the term set correctly to Term TextValue NumericUnits Range$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnitsWithRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        val testResultValue =
                MyRecordSerenityHelpers.EXPECTED_TEST_RESULT.getOrFail<TestResultValue>()
        val lowerRange = testResultValue.range!!.minimumText
        val upperRange = testResultValue.range!!.maximumText
        assertEquals("${testResultValue.term}: ${testResultValue.textValue} " +
                "${testResultValue.numericUnits} (normal range: $lowerRange - $upperRange)",
                result.response.testResults.data.first().description)
    }
}

