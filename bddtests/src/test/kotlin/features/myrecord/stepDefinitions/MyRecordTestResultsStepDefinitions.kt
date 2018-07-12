package features.myrecord.stepDefinitions

import cucumber.api.java.en.*
import features.myrecord.mockData.TestResultsData
import mocking.tpp.models.Error
import mocking.tpp.models.TestResultsViewReply
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse
import java.time.LocalDateTime
import java.time.OffsetDateTime
import java.time.format.DateTimeFormatter

open class MyRecordTestResultsStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Given("^the GP Practice has multiple test results for (.*)$")
    fun givenTheGpPracticeHasMultipleTestResultsFor(getService:String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getMultipleTestResultsData())
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    testResultsViewRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!).respondWithSuccess(TestResultsData.getMultipleTppTestResultsData())
                }
            }
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with no ranges for (.*)$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesFor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithNoRanges())
                }
            }
            "TPP" -> {
            }
        }
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
        }
    }

    @Given("^the GP Practice has test results enabled and a single test result exists with no child values or range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesOrRange())
        }
    }

    @Given("^the GP Practice has a single test result with no child values and range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesAndARange())
        }
    }

    @Given("^I do not have access to test results for (.*)$")
    fun givenIDoNotHaveAccessToTestResultsFor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("^I have no test results for (.*)$")
    fun givenIHaveNoTestResultsFor(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithSuccess(TestResultsData.getDefaultTestResultsModel())
                }
            }
            "TPP" -> {

            }
        }
    }

    @Given("^an error occurred retrieving the test results from (.*)$")
    fun givenAnErrorOccurredRetrievingTestResultsFrom(getService: String) {
        setPatientToDefaultFor(getService)
        when(getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithNonDataAccessException()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    testResultsViewRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!).respondWithServiceNotAvailableException()
                }
            }
        }
    }

    @But("^the GP Practice has disabled test results functionality for (.*)$")
    fun butTheGPPracticeHasDisabledTestResultsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        when (getService) {
            "EMIS" -> {
                mockingClient.forEmis {
                    testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient).respondWithExceptionWhenNotEnabled()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    testResultsViewRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!)
                    testResultsViewRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!)
                            .respondWithError(Error("6", "Requested record access is disabled by the practice", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                }
            }
        }
    }
    @When("^I get the users test results$")
    fun whenIGetTheUsersMyRecordData()
    {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getMyRecord(null)

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive (.*) test results as part of the my record object$")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(count, result.response.testResults.data.count())
    }

    @Then("^I receive the test result with term set correctly to Term$")
    fun thenIReceiveATestResultWithTermSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count", result.response.testResults.data.first().description)
    }

    @Then("^the line item displays text value and range$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectlyIncludingRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Platelet count: 5.9 x10^9/L (normal range: 3.6 - 10)", result.response.testResults.data.first().testResultLineItems.first())
    }

    @Then("^the line item value is set correctly$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Platelet count: 5.9 x10^9/L", result.response.testResults.data.first().testResultLineItems.first())
    }

    @Then("^I receive line items for each child value$")
    fun thenIReceiveATestResultWithLineItemsForEachChildValue() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(2, result.response.testResults.data.first().testResultLineItems.count())
    }

    @Then("^I receive a single test result with the term set correctly to Term TextValue NumericUnits$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnits() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count: 5.58 x10^9/L", result.response.testResults.data.first().description)
    }

    @Then("^I receive the term set correctly to Term TextValue NumericUnits Range$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnitsWithRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals("Neutrophil count: 5.58 x10^9/L (normal range: 1.7 - 6)", result.response.testResults.data.first().description)
    }

    @And("^the flag informing that the patient has access to the test results data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.testResults.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the test results data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        Assert.assertEquals(value, result.response.testResults.hasErrored)
    }
}

