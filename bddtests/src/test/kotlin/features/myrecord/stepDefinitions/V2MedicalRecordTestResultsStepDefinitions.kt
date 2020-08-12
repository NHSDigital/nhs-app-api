package features.myrecord.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.myrecord.factories.TestResultsFactory
import features.myrecord.factories.TestResultsFactoryVision
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.gpMedicalRecord.TestResultsPage
import pages.myrecord.MyRecordTestResultDetailPage
import utils.SerenityHelpers

open class V2MedicalRecordTestResultsStepDefinitions {

    private lateinit var testResultsPage: TestResultsPage
    private lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage
    private lateinit var errorPage: ErrorPage

    @Given("^an error occurred retrieving the test results")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        val testResultsFactory = TestResultsFactoryVision()
        testResultsFactory.errorRetrieving(SerenityHelpers.getPatient())
    }

    @When("I click a test result - Medical Record v2$")
    fun whenIClickATestResultV2() {
        testResultsPage.clickTestResult()
    }
    @When("^there is a corrupted test results response returned$")
    fun thereIsACorruptedTestResultsResponse() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        TestResultsFactory.getForSupplier(gpSystem).respondWithACorruptedResponse(SerenityHelpers.getPatient())

    }

    @Then("^I see one test result with one value - Medical Record v2$")
    fun thenISeeOneTestResultWithOneValueV2() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = childElements.map { element -> element.textValue }[0]
        val commentElements = testResultsPage.getTestResultParentComments()
        val commentElementsMapped = commentElements.map { element -> element.textValue }[0]

        assertEquals("Expected test result", 1, mainElements.size)
        assertEquals("Expected test result", "15 May 2006", mainElements[0].label)
        assertEquals("Expected child test result", 1, childElements.size)
        assertEquals("Expected child test result", "Test result component term: 5.58 x10^9/L", childElementsMapped)
        assertEquals("Expected test result comment", 1, commentElements.size)
        assertEquals("Expected test result comment", "Test result comment", commentElementsMapped)
    }

    @Then("^I see one test result with one value and a range - Medical Record v2$")
    fun thenISeeOneTestResultWithOneValueAndARangeV2() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = childElements.map { element -> element.textValue }[0]
        val commentElements = testResultsPage.getTestResultParentComments()
        val commentElementsMapped = commentElements.map { element -> element.textValue }[0]

        assertEquals("Expected test result", 1, mainElements.size)
        assertEquals("Expected test result", "15 May 2006", mainElements[0].label)
        assertEquals("Expected child test result", 1, childElements.size)
        assertEquals("Expected child test result", "Test result component term: 5.58 x10^9/L (normal range: " +
                "3.6 - 10)", childElementsMapped)
        assertEquals("Expected test result comment", 1, commentElements.size)
        assertEquals("Expected test result comment", "Test result comment", commentElementsMapped)
    }

    @Then("^I see one test result with multiple child values - Medical Record v2$")
    fun thenISeeOneTestResultWithMultipleChildValuesV2() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()

        Assert.assertTrue("Expected test result equal to or greater than 1",
                mainElements.isNotEmpty())
        Assert.assertTrue("Expected child test result greater than 1",
                childElements.size > 1)
    }

    @Then("^I see test results with multiple child values some of which have ranges - Medical Record v2$")
    fun thenISeeTestResultsWithMultipleChildValuesSomeOfWhichHaveRangesV2() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = testResultsPage.getTestResultChildren().map { element ->
            element.textValue }[1]

        Assert.assertTrue("Expected test result equal to or greater than 1",
                mainElements.isNotEmpty())
        Assert.assertTrue("Expected child test result greater than 1",
                childElements.size > 1)
        assertEquals("Expected child test result", "Test result component term: 5.58 x10^9/L (normal range: " +
                "3.6 - 10)", childElementsMapped)
    }

    @Then("^I see (.*) test results - Medical Record v2$")
    fun thenISeeMultipleTestResultsV2(count: Int) {
        assertEquals("Expected test results", count, testResultsPage.getTestResultsElements().count())
    }

    @Then("^I see the correct number of test results for current the supplier - Medical Record v2$")
    fun thenISeeTheCorrectNumberOfTestResultsV2() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        if(gpSystem == Supplier.EMIS)
            thenISeeMultipleTestResultsV2(emisDefaultTestResultsCount)
        else if(gpSystem == Supplier.TPP)
            thenISeeMultipleTestResultsV2(tppDefaultTestResultsCount)
    }

    @Then("^The third test result record has an unknown date - Medical Record v2$")
    fun thenTheThirdResultHasAnUnknownDateV2() {
        val dateLabel = testResultsPage.getTestResultsElements()[2].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }

    @Then("^I see the test results content - Medical Record v2$")
    fun thenISeeTheTestResultsContentV2() {
        myRecordDetailedTestResultPage.assertContentMedicalRecordV2()
    }

    @Then("^there are no wrongly displayed HTML entities - Medical Record v2$")
    fun thereAreNoWronglyDisplayedHTMLEntitiesV2() {
        myRecordDetailedTestResultPage.assertContentWithNoWronglyDisplayedHTMLEntities()
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

    companion object {
        private const val emisDefaultTestResultsCount = 3
        private const val tppDefaultTestResultsCount = 6
    }
}
