package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.TestResultsData
import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordTestResultsStepDefinitions {

    @Steps
    val mockingClient = MockingClient.instance
    val HTTP_EXCEPTION = "HttpException"

    @Given("the GP Practice has multiple test results")
    fun givenTheGpPracticeHasMultipleTestResults() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
        }
    }

    @Given("the GP Practice has a single test result with multiple child values with no ranges")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRanges() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithNoRanges())
        }
    }

    @Given("the GP Practice has a single test result with single child values with no ranges")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRanges() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
        }
    }

    @Given("the GP Practice has a single test result with multiple child values with ranges")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRanges() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
        }
    }

    @Given("the GP Practice has a single test result with single child value with A range")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithaRanges() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
        }
    }

    @Given("the GP Practice has test results enabled and a single test result exists with no child values or range")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRange() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesOrRange())
        }
    }

    @Given("the GP Practice has test results enabled and a single test result exists with no child values and a range")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARange() {

        mockingClient.forEmis {
            testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesAndARange())
        }
    }

    @When("I get the users test results")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive \"(.*)\" test results as part of the my record object")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.testResults.data.count())
    }

    @Then("I receive the test result with term set correctly to Term")
    fun thenIReceiveATestResultWithTermSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count", result.response.testResults.data.first().term)
    }

    @Then("the line item displays text value and range")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectlyIncludingRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Platelet count: 5.9 x10^9/L (normal range: 3.6 - 10)", result.response.testResults.data.first().testResultLineItems.first())
    }

    @Then("the line item value is set correctly")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Platelet count: 5.9 x10^9/L", result.response.testResults.data.first().testResultLineItems.first())
    }

    @Then("I receive line items for each child value")
    fun thenIReceiveATestResultWithLineItemsForEachChildValue() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(2, result.response.testResults.data.first().testResultLineItems.count())
    }

    @Then("I receive a single test result with the term set correctly to Term TextValue NumericUnits")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnits() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count: 5.58 x10^9/L", result.response.testResults.data.first().term)
    }

    @Then("I receive the term set correctly to Term TextValue NumericUnits Range")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnitsWithRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count: 5.58 x10^9/L (normal range: 1.7 - 6)", result.response.testResults.data.first().term)
    }
}

