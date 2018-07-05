package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.mockData.TestResultsData
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

    @Given("the GP Practice has multiple test results for (.*)")
    fun givenTheGpPracticeHasMultipleTestResultsFor(getService:String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has a single test result with multiple child values with no ranges for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithNoRanges())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has a single test result with single child values with no ranges for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has a single test result with multiple child values with ranges for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has a single test result with single child value with A range for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has test results enabled and a single test result exists with no child values or range for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesOrRange())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("the GP Practice has a single test result with no child values and range for (.*)")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesAndARange())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("I do not have access to test results for (.*)")
    fun givenIDoNotHaveAccessToTestResultsFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("I have no test results for (.*)")
    fun givenIHaveNoTestResultsFor(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("an error occurred retrieving the test results from (.*)")
    fun givenAnErrorOccurredRetrievingTestResultsFrom(getService: String) {
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(MockDefaults.patient).respondWithNonDataAccessException()
                }
            }
            "TPP" -> {

            }
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

