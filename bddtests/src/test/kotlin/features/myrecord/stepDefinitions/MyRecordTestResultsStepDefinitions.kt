package features.myrecord.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.TestResultsFactory
import mocking.data.myrecord.TestResultsData
import net.serenitybdd.core.Serenity
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordTestResultDetailPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

private const val NUMBER_OF_TEST_RESULTS_EQUALS_FOUR = 4
open class MyRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage
    lateinit var errorPage: ErrorPage
    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^an error occurs retrieving the test result detail$")
    fun givenAnErrorOccursGettingTestResultDetailForTpp() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithServiceNotAvailableException()
        }
    }

    @Given("^the test result details are retrieved successfully$")
    fun successGettingTestResultDetailForTpp() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_FOUR))
        }
    }

    @Given("^the GP Practice has test result details$")
    fun givenTestResultDetailIsRetrievedSuccessfully() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(this@MyRecordTestResultsStepDefinitions.patient.tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetail())
        }
    }

    @Given("^the GP Practice has six test results for (.*)$")
    fun givenTheGpPracticeHasSixTestResultsFor(getService: String) {
        setPatientToDefaultFor(getService)
        TestResultsFactory.getForSupplier(getService).enabledWithRecords(patient)
    }

    @Given("^the GP Practice has a single test result with multiple child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData
                            .getSingleTestResultWithMultipleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
        }
    }

    @Given("^the GP Practice has test results enabled " +
            "and a single test result exists with no child values or range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesOrRange())
        }
    }

    @Given("^the GP Practice has a single test result with no child values and range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(this@MyRecordTestResultsStepDefinitions.patient)
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesAndARange())
        }
    }

    @Given("^I do not have access to test results for (.*)$")
    fun givenIDoNotHaveAccessToTestResultsFor(getService: String) {
        setPatientToDefaultFor(getService)
        TestResultsFactory.getForSupplier(getService).noAccess(patient)
    }

    @Given("^I have no test results for (.*)$")
    fun givenIHaveNoTestResultsFor(getService: String) {
        setPatientToDefaultFor(getService)
        TestResultsFactory.getForSupplier(getService).enabledWithBlankRecord(patient)
    }

    @Given("^an error occurred retrieving the test results from (.*)$")
    fun givenAnErrorOccurredRetrievingTestResultsFrom(getService: String) {
        setPatientToDefaultFor(getService)
        TestResultsFactory.getForSupplier(getService).errorRetrieving(patient)
    }

    @But("^the GP Practice has disabled test results functionality for (.*)$")
    fun butTheGPPracticeHasDisabledTestResultsFunctionalityFor(getService: String) {
        setPatientToDefaultFor(getService)
        TestResultsFactory.getForSupplier(getService).disabled(patient)
    }

    @When("^I get the users test results$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()

            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("I click a test result$")
    fun whenIClickATestResult() {
        myRecordInfoPage.testResults.clickFirst()
    }

    @Then("^I receive (.*) test results as part of the my record object$")
    fun thenIReceiveATestResultsObject(count: Int) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(count, result.response.testResults.data.count())
    }

    @Then("^I receive the test result with term set correctly to Term$")
    fun thenIReceiveATestResultWithTermSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Neutrophil count", result.response.testResults.data.first().description)
    }

    @Then("^the line item displays text value and range$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectlyIncludingRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Child LineItem Description does not match",
                "Platelet count: 5.9 x10^9/L (normal range: 3.6 - 10)",
                result.response.testResults.data.first().testResultChildLineItems.first().description)
    }

    @Then("^the line item value is set correctly$")
    fun thenIReceiveATestResultWithLineItemValueSetCorrectly() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Child LineItem Description does not match",
                "Platelet count: 5.9 x10^9/L",
                result.response.testResults.data.first().testResultChildLineItems.first().description)
    }

    @Then("^I receive line items for each child value$")
    fun thenIReceiveATestResultWithLineItemsForEachChildValue() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Expected two ChildLineItems in TestResult",
                2,
                result.response.testResults.data.first().testResultChildLineItems.count())
    }

    @Then("^I receive a single test result with the term set correctly to Term TextValue NumericUnits$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnits() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Neutrophil count: 5.58 x10^9/L", result.response.testResults.data.first().description)
    }

    @Then("^I receive the term set correctly to Term TextValue NumericUnits Range$")
    fun thenIReceiveASingleTestWithTheTermSetCorrectlyToTermTextValueAndNumericUnitsWithRange() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals("Neutrophil count: 5.58 x10^9/L (normal range: 1.7 - 6)",
                result.response.testResults.data.first().description)
    }

    @And("^the flag informing that the patient has access to the test results data is set to \"(.*)\"$")
    fun andHasAccessToMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.testResults.hasAccess)
    }

    @And("^the flag informing that there was an error retrieving the test results data is set to \"(.*)\"$")
    fun andHasErrorsWhenRetrievingMedicationsDataIsSetTo(value: Boolean) {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(value, result.response.testResults.hasErrored)
    }

    @Then("I see the appropriate error message for retrieving test result detail")
    fun thenISeeTheAppropriateErrorMessageForAMyRecordServerError() {

        val pageHeader = myRecordDetailedTestResultPage.serverErrorPageHeader
        val header = myRecordDetailedTestResultPage.serverErrorHeader
        val message = myRecordDetailedTestResultPage.serverErrorMessage

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertPageHeader(pageHeader)
                .assertNoRetryButton()
    }

    @Then("I select a test result")
    fun thenISelectATestResult() {
        myRecordInfoPage.testResults.toggleShrub()
        myRecordInfoPage.testResults.clickFirst()
    }

    @Then("I am on the test result detail page")
    fun thenISeeTheTestResultDetailPage() {
        myRecordDetailedTestResultPage.testResultDetailsHeader.assertSingleElementPresent().assertIsVisible()
        myRecordDetailedTestResultPage.backButton.assertIsVisible()
    }

    @Then("I click the test result detail back button")
    fun thenIClickTheTestResultDetailBackButton() {
        myRecordDetailedTestResultPage.clickBackToMyRecordButton()
    }

    @When("^I click the test result section$")
    fun whenIClickTheTestResultSection() {
        myRecordInfoPage.testResults.toggleShrub()
    }

    @Then("^I see one test result with one value$")
    fun thenISeeOneTestResultWithOneValue() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with one value and a range$")
    fun thenISeeOneTestResultWithOneValueAndARange() {
        assertEquals("Expected test result", 1, myRecordInfoPage.testResults.allRecordItems().size)
        assertEquals("Expected child test result", 1, myRecordInfoPage.getTestResultChildCount())
    }

    @Then("^I see one test result with multiple child values$")
    fun thenISeeOneTestResultWithMultipleChildValues() {
        Assert.assertTrue("Expected test result equal to or less than 1, but was" +
                "${myRecordInfoPage.testResults.allRecordItems().size}",
                myRecordInfoPage.testResults.allRecordItems().size >= 1)
        Assert.assertTrue("Expected child test result equal to or less than 1, but was " +
                "${myRecordInfoPage.getTestResultChildCount()}",
                myRecordInfoPage.getTestResultChildCount() > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRanges() {
        Assert.assertTrue("Expected test result equal to or greater than 1, but was" +
                "${myRecordInfoPage.testResults.allRecordItems().size}"
                , myRecordInfoPage.testResults.allRecordItems().size >= 1)
        Assert.assertTrue("Expected child test result equal to or greater than 1, but was " +
                "${myRecordInfoPage.getTestResultChildCount()}",
                myRecordInfoPage.getTestResultChildCount() > 1)
    }

    @Then("^I see the test result heading for (.*)$")
    fun thenISeeTheTestResultHeading(getService: String) {
        val header = when (getService) {
            "TPP" -> {
                "Test results (past 6 months)"
            }
            else -> {
                "Test results"
            }
        }
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @Then("^I see the test result section collapsed$")
    fun thenISeeTheTestResultSectionCollapsed() {
        Assert.assertFalse(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see test result information$")
    fun thenISeeTestResultInformation() {
        Assert.assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        assertEquals("Expected test results", count, myRecordInfoPage.testResults.allRecordItems().count())
    }

    @Then("I see the my record page scrolled to the test result section")
    fun thenISeeMyRecordPageScrolledToTestResultSection() {
        Assert.assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }
}

