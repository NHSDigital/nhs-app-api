package features.myrecord.stepDefinitions

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
import pages.gpMedicalRecord.TestResultsPage
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordTestResultDetailPage
import utils.SerenityHelpers

private const val NUMBER_OF_TEST_RESULTS_EQUALS_FOUR = 4
open class GpMedicalRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var errorPage: ErrorPage
    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var testResultsPage: TestResultsPage
    lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage

    @Given("^I do not have access to test results - GP Medical Record$")
    fun givenIDoNotHaveAccessToTestResultsGpMedicalRecord() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).noAccess(SerenityHelpers.getPatient())
    }

    @Given("^I have no test results - GP Medical Record$")
    fun givenIHaveNoTestResultsGpMedicalRecord() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has a single test result with single child values with no ranges for" +
            " EMIS - GP Medical Record$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValuesWithNoRangesForGpMedicalRecord() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with single child value with A range for " +
            "EMIS - GP Medical Record$")
    fun givenTheGpPracticeHasASingleTestResultWithSingleChildValueWithRangesForGpMedicalRecord() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithSingleChildValuesWithARange())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with no ranges for " +
            "EMIS - GP Medical Record$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithNoRangesForGpMedicalRecord() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData
                            .getSingleTestResultWithMultipleChildValuesWithNoRanges())
        }
    }

    @Given("^the GP Practice has a single test result with multiple child values with ranges for" +
            " EMIS - GP Medical Record$")
    fun givenTheGpPracticeHasASingleTestResultWithMultipleChildValuesWithRangesForGpMedicalRecord() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getSingleTestResultWithMultipleChildValuesWithRanges())
        }
    }

    @Given("^the EMIS GP Practice has two test results where the second record has no date - GP Medical Record$")
    fun givenTheEmisGpPracticeHasATestResultWithNoDateGpMedicalRecord() {
        mockingClient.forEmis {
            myRecord.testResultsRequest(SerenityHelpers.getPatient())
                    .respondWithSuccess(TestResultsData.getTwoTestResultsWhereTheSecondRecordHasNoDate())
        }
    }

    @Then("^I see one test result with one value - GP Medical Record$")
    fun thenISeeOneTestResultWithOneValueGpMedicalRecord() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = testResultsPage.getTestResultChildren().map { element ->
            element.textValue }[0]
        assertEquals("Expected test result", 1, mainElements.size)
        assertEquals("Expected test result", "15 May 2006", mainElements[0].label)
        assertEquals("Expected child test result", 1, childElements.size)
        assertEquals("Expected child test result", "Basophil count: 5.58 x10^9/L", childElementsMapped)
    }

    @Then("^I see one test result with one value and a range - GP Medical Record$")
    fun thenISeeOneTestResultWithOneValueAndARangeGpMedicalRecord() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = testResultsPage.getTestResultChildren().map { element ->
            element.textValue }[0]
        assertEquals("Expected test result", 1, mainElements.size)
        assertEquals("Expected test result", "15 May 2006", mainElements[0].label)
        assertEquals("Expected child test result", 1, childElements.size)
        assertEquals("Expected child test result", "Basophil count: 5.58 x10^9/L (normal range: " +
                "3.6 - 10)", childElementsMapped)
    }

    @Then("^I see one test result with multiple child values - GP Medical Record$")
    fun thenISeeOneTestResultWithMultipleChildValuesGpMedicalRecord() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        Assert.assertTrue("Expected test result equal to or greater than 1",
                mainElements.isNotEmpty())
        Assert.assertTrue("Expected child test result greater than 1",
                childElements.size > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges - GP Medical Record$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRangesGpMedicalRecord() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = testResultsPage.getTestResultChildren().map { element ->
            element.textValue }[1]
        Assert.assertTrue("Expected test result equal to or greater than 1",
                mainElements.isNotEmpty())
        Assert.assertTrue("Expected child test result greater than 1",
                childElements.size > 1)
        assertEquals("Expected child test result", "Basophil count: 5.58 x10^9/L (normal range: " +
                "3.6 - 10)", childElementsMapped)
    }

    @Then("^I see (.*) test results - GP Medical Record$")
    fun thenISeeMultipleTestResultsGpMedicalRecord(count: Int) {
        assertEquals("Expected test results", count, testResultsPage.getTestResultsElements().count())
    }

    @Then("^The second test result record has an unknown date - GP Medical Record$")
    fun thenTheSecondResultHasAnUnknownDateGpMedicalRecord() {
        val dateLabel = testResultsPage.getTestResultsElements()[1].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }

    @Then("^I see the test result content - GP Medical Record$")
    fun thenISeeTheTestResulsContent() {
        myRecordDetailedTestResultPage.assertContentGpMedicalRecord()
    }

    @Given("^the GP Practice has multiple test results - GP Medical Record$")
    fun givenTheGpPracticeHasSixTestResultsGpMedicalRecord() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).enabledWithRecords(SerenityHelpers.getPatient())
    }

    @When("I click a test result - GP Medical Record$")
    fun whenIClickATestResultGpMedicalRecord() {
        testResultsPage.clickTestResult()
    }

    @Given("^an error occurs retrieving the test result detail - GP Medical Record$")
    fun givenAnErrorOccursGettingTestResultDetailForTppGpMedicalRecord() {
        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithServiceNotAvailableException()
        }
    }

    @Given("^the GP Practice has test result details - GP Medical Record$")
    fun givenTestResultDetailIsRetrievedSuccessfullyGpMedicalRecord() {

        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData.getTestResultDetail())
        }
    }

    @Given("^the test result details are retrieved successfully - GP Medical Record$")
    fun successGettingTestResultDetailForTppGpMedicalRecord() {
        mockingClient.forTpp {
            myRecord.testResultsDetailRequest(SerenityHelpers.getPatient().tppUserSession!!,
                    TestResultsData.mockTestResultId)
                    .respondWithSuccess(TestResultsData
                            .getMultipleTestResultsForTpp(NUMBER_OF_TEST_RESULTS_EQUALS_FOUR))
        }
    }
}
