package features.myrecord.stepDefinitions

import config.Config
import constants.DateTimeFormats
import constants.Supplier
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
import pages.myrecord.MedicalRecordV1Page
import pages.myrecord.MyRecordTestResultDetailPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter
private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ZERO = 0
private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ONE = 1
private const val NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO = 2
private const val NUMBER_OF_TEST_RESULTS_EQUALS_FOUR = 4
open class V1MedicalRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    private lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage
    lateinit var errorPage: ErrorPage
    lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Given("^an error occurs retrieving the test result detail$")
    fun givenAnErrorOccursGettingTestResultDetailForTpp() {
        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithServiceNotAvailableException()
        }
    }

    @Given("^the test result details are retrieved successfully$")
    fun successGettingTestResultDetailForTpp() {
        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_FOUR))
        }
    }

    @Given("^the GP Practice has test result details$")
    fun givenTestResultDetailIsRetrievedSuccessfully() {
        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetail())
        }
    }

    @Given("^the GP Practice has test result details with HTML entities$")
    fun givenTestResultDetailWithHTMLEntitiesIsRetrievedSuccessfully() {
        mockingClient.forTpp {
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
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(
                                    NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO, false))
        }
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesForEmis() {
        mockingClient.forEmis {
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

    @Given("^the EMIS GP Practice has two test results where the second record has no date$")
    fun givenTheEmisGpPracticeHasATestResultWithNoDate() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getTwoTestResultsWhereTheSecondRecordHasNoDate())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesFor() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_TWO))
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesFor() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ONE))
        }
    }

    @Given("^the GP Practice has test results enabled " +
            "and a single test result exists with no child values or range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesOrRangeFor() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getTestResultWithChildValueCountAndRangePresent(
                                    NUMBER_OF_CHILD_VALUES_COUNT_EQUALS_ZERO, false))
        }
    }

    @Given("^the GP Practice has a single test result with no child values and range for EMIS$")
    fun givenTheGpPracticeHasASingleTestResultWithNoChildValuesAndARangeFor() {
        mockingClient.forEmis {
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

    @When("I click a test result - Medical Record v1$")
    fun whenIClickATestResult() {
        medicalRecordV1Page.testResults.clickFirst()
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

    @Then("I select a test result - Medical Record v1")
    fun thenISelectATestResultV1() {
        medicalRecordV1Page.testResults.toggleShrub()
        medicalRecordV1Page.testResults.clickFirst()
    }

    @Then("^I see the test result content - Medical Record v1$")
    fun thenISeeTheTestResulsContentV1() {
        myRecordDetailedTestResultPage.assertContent()
    }

    @Then("^there are no wrongly displayed HTML entities - Medical Record v1$")
    fun thenThereAreNoWronglyDisplayedHTMLEntitiesV1() {
        myRecordDetailedTestResultPage.assertContentWithNoWronglyDisplayedHTMLEntities()
    }

    @Then("^I see test results with multiple child values some of which have ranges - Medical Record v1$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRangesV1() {
        Assert.assertTrue("Expected test result equal to or greater than 1, but was" +
                "${medicalRecordV1Page.testResults.allRecordItems().size}"
                , medicalRecordV1Page.testResults.allRecordItems().isNotEmpty())
        Assert.assertTrue("Expected child test result equal to or greater than 1, but was " +
                "${medicalRecordV1Page.getTestResultChildCount()}",
                medicalRecordV1Page.getTestResultChildCount() > 1)
    }

    @Then("^I see test result information - Medical Record v1$")
    fun thenISeeTestResultInformationV1() {
        val gpSystem = SerenityHelpers.getGpSupplier()

        if (gpSystem == Supplier.VISION) {
            Assert.assertTrue(medicalRecordV1Page.isVisionTestResultsLinkVisible())
        } else {
            Assert.assertTrue(medicalRecordV1Page.isTestResultsTextMsgVisible())
        }
    }

    @Then("^I see (.*) test results - Medical Record v1$")
    fun thenISeeMultipleTestResultsV1(count: Int) {
        assertEquals("Expected test results", count, medicalRecordV1Page.testResults.allRecordItems().count())
    }

    @Then("^The second test result record has an unknown date - Medical Record v1$")
    fun thenTheSecondResultHasAnUnknownDateV1() {
        val dateLabel = medicalRecordV1Page.testResults.allRecordItems()[1].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }

    @Then("^I see the expected test results displayed - Medical Record v1$")
    fun thenISeeTheExpectedTestResultsDisplayedV1() {
        val expectedTestResults = TestResultsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedTestResults()

        val onScreenTestResults = medicalRecordV1Page.testResults.allRecordItems()

        assertEquals(expectedTestResults.count(), onScreenTestResults.count())

        for (i in onScreenTestResults.indices) {
            assertEquals(
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
               assertEquals(expectedTestResults[i].associatedTexts[j], onScreenTestResults[i].bodyElements[j])
            }
        }
    }
}

