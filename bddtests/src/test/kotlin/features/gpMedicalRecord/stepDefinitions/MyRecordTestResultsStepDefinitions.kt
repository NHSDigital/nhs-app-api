package features.gpMedicalRecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.myrecord.factories.TestResultsFactory
import features.sharedSteps.BrowserSteps
import mocking.data.myrecord.TestResultsData
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.gpMedicalRecord.TestResultsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers

open class MyRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    lateinit var errorPage: ErrorPage
    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var testResultsPage: TestResultsPage

    @Given("^I do not have access to test results - GP Medical Record$")
    fun givenIDoNotHaveAccessToTestResultsGpRecordGpRecord() {
        val getService = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(getService).noAccess(SerenityHelpers.getPatient())
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
    fun thenISeeOneTestResultWithOneValueGpRecord() {
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
    fun thenISeeOneTestResultWithOneValueAndARangeGpRecord() {
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
    fun thenISeeOneTestResultWithMultipleChildValuesGpRecord() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        Assert.assertTrue("Expected test result equal to or greater than 1",
                mainElements.isNotEmpty())
        Assert.assertTrue("Expected child test result greater than 1",
                childElements.size > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges - GP Medical Record$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRangesGpRecord() {
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
    fun thenISeeMultipleTestResultsGpRecord(count: Int) {
        assertEquals("Expected test results", count, testResultsPage.getTestResultsElements().count())
    }

    @Then("^The second test result record has an unknown date - GP Medical Record$")
    fun thenTheSecondResultHasAnUnknownDateGpRecord() {
        val dateLabel = testResultsPage.getTestResultsElements()[1].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }
}

