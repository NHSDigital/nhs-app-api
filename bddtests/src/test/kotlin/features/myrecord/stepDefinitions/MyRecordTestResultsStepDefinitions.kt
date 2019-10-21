package features.myrecord.stepDefinitions

import config.Config
import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.TestResultsFactory
import features.sharedSteps.BrowserSteps
import mocking.data.myrecord.TestResultsData
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordTestResultDetailPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_TEST_RESULTS_EQUALS_FOUR = 4
open class MyRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage
    lateinit var errorPage: ErrorPage
    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^an error occurs retrieving the test result detail$")
    fun givenAnErrorOccursGettingTestResultDetailForTpp() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithServiceNotAvailableException()
        }
    }

    @Given("^the test result details are retrieved successfully$")
    fun successGettingTestResultDetailForTpp() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_FOUR))
        }
    }

    @Given("^the GP Practice has test result details$")
    fun givenTestResultDetailIsRetrievedSuccessfully() {
        setPatientToDefaultFor("TPP")

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetail())
        }
    }

    @Given("^the GP Practice has six test results$")
    fun givenTheGpPracticeHasSixTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        TestResultsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has a single test result with multiple child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getSingleTestResultWithMultipleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
        }
    }

    @Given("^the EMIS GP Practice has two test results where the second record has no date$")
    fun givenTheEmisGpPracticeHasATestResultWithNoDate() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getTwoTestResultsWhereTheSecondRecordHasNoDate())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
        }
    }

    @Given("^the GP Practice has test results enabled " +
            "and a single test result exists with no child values or range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesOrRange())
        }
    }

    @Given("^the GP Practice has a single test result with no child values and range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor() {
        setPatientToDefaultFor("EMIS")
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithNoChildValuesAndARange())
        }
    }

    @Given("^I do not have access to test results$")
    fun givenIDoNotHaveAccessToTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        TestResultsFactory.getForSupplier(gpSystem).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^I have no test results$")
    fun givenIHaveNoTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        TestResultsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^an error occurred retrieving the test results$")
    fun givenAnErrorOccurredRetrievingTestResults() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        TestResultsFactory.getForSupplier(gpSystem).errorRetrieving(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled test results functionality$")
    fun butTheGPPracticeHasDisabledTestResultsFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(gpSystem)
        TestResultsFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @When("I click a test result$")
    fun whenIClickATestResult() {
        myRecordInfoPage.testResults.clickFirst()
    }

    @When("^I click the test result section$")
    fun whenIClickTheTestResultSection() {
        myRecordInfoPage.testResults.toggleShrub()
    }

    @When("^I enter url address for test results detail directly into the url$")
    fun whenIEnterUrlAddressForTestResultsDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/test-results-detail"
        browser.browseTo(fullUrl)
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

    @Then("I click the test result detail back")
    fun thenIClickTheTestResultDetailBackLink() {
        myRecordDetailedTestResultPage.clickBackToMyRecord()
    }

    @Then("^I see the test result content$")
    fun thenISeeTheTestResulsContent() {
        myRecordDetailedTestResultPage.assertContent()
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

    @Then("^I see the test result heading$")
    fun thenISeeTheTestResultHeading() {
        val header = when (SerenityHelpers.getGpSupplier()) {
            "TPP" -> {
                "Test results (past 6 months)"
            }
            else -> {
                "Test results"
            }
        }
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @Then("^I see test result information$")
    fun thenISeeTestResultInformation() {
        val gpSystem = SerenityHelpers.getGpSupplier()

        if (gpSystem == "VISION") {
            Assert.assertTrue(myRecordInfoPage.isVisionTestResultsLinkVisible())
        } else {
            Assert.assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
        }
    }

    @Then("^I see (.*) test results$")
    fun thenISeeMultipleTestResults(count: Int) {
        assertEquals("Expected test results", count, myRecordInfoPage.testResults.allRecordItems().count())
    }

    @Then("^The second test result record has an unknown date$")
    fun thenTheSecondResultHasAnUnknownDate() {
        val dateLabel = myRecordInfoPage.testResults.allRecordItems()[1].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }

    @Then("I see the my record page scrolled to the test result section")
    fun thenISeeMyRecordPageScrolledToTestResultSection() {
        Assert.assertTrue(myRecordInfoPage.isTestResultsTextMsgVisible())
    }


    @Then("^I see the expected test results displayed$")
    fun thenISeeTheExpectedTestResultsDisplayed() {

        val gpSystem = SerenityHelpers.getGpSupplier()
        val expectedTestResults = TestResultsFactory
                .getForSupplier(gpSystem)
                .getExpectedTestResults()

        val onScreenTestResults = myRecordInfoPage.testResults.allRecordItems()

        Assert.assertEquals(expectedTestResults.count(), onScreenTestResults.count())

        for (i in onScreenTestResults.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            expectedTestResults[i].date.value
                    ),
                    LocalDate.parse(
                            onScreenTestResults[i].label,
                            DateTimeFormatter.ofPattern(
                                    DateTimeFormats.frontendBasicDateFormat)
                    )
            )

            for (j in expectedTestResults[i].associatedTexts.indices) {
               Assert.assertEquals(expectedTestResults[i].associatedTexts[j], onScreenTestResults[i].bodyElements[j])
            }
        }
    }
}

