package features.myrecord.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.emis.testResults.TestResultValue
import net.serenitybdd.core.Serenity
import org.junit.Assert.assertEquals
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class V1MedicalRecordTestResultsStepDefinitionsBackend {

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

