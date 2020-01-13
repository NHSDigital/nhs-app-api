package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.TestResultsFactoryVision
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.gpMedicalRecord.TestResultsPage
import pages.myrecord.MyRecordTestResultDetailPage
import utils.SerenityHelpers

open class V2MedicalRecordTestResultsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var testResultsPage: TestResultsPage
    private lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage

    @Given("^an error occurred retrieving the test results")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        val testResultsFactory = TestResultsFactoryVision()
        testResultsFactory.errorRetrieving(SerenityHelpers.getPatient())
    }

    @When("I click a test result - Medical Record v2$")
    fun whenIClickATestResultV2() {
        testResultsPage.clickTestResult()
    }

    @Then("^I see one test result with one value - Medical Record v2$")
    fun thenISeeOneTestResultWithOneValueV2() {
        val mainElements = testResultsPage.getTestResultsElements()
        val childElements = testResultsPage.getTestResultChildren()
        val childElementsMapped = testResultsPage.getTestResultChildren().map { element ->
            element.textValue }[0]

        assertEquals("Expected test result", 1, mainElements.size)
        assertEquals("Expected test result", "15 May 2006", mainElements[0].label)
        assertEquals("Expected child test result", 1, childElements.size)
        assertEquals("Expected child test result", "Basophil count: 5.58 x10^9/L", childElementsMapped)
    }

    @Then("^I see one test result with one value and a range - Medical Record v2$")
    fun thenISeeOneTestResultWithOneValueAndARangeV2() {
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
        assertEquals("Expected child test result", "Basophil count: 5.58 x10^9/L (normal range: " +
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

    @Then("^The second test result record has an unknown date - Medical Record v2$")
    fun thenTheSecondResultHasAnUnknownDateV2() {
        val dateLabel = testResultsPage.getTestResultsElements()[1].label
        assertEquals("Test result date", "Unknown Date", dateLabel)
    }

    @Then("^I see the test result content - Medical Record v2$")
    fun thenISeeTheTestResulsContentV2() {
        myRecordDetailedTestResultPage.assertContentMedicalRecordV2()
    }

    companion object {
        private const val emisDefaultTestResultsCount = 2
        private const val tppDefaultTestResultsCount = 6
    }
}
