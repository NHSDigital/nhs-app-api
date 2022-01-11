package features.myrecord.stepDefinitions

import constants.Supplier
import features.myrecord.factories.TestResultsFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.PageUrl
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.assertIsVisible
import pages.gpMedicalRecord.TestResultsPage
import pages.myrecord.MyRecordChooseTestResultYearPage
import pages.myrecord.MyRecordTestResultDetailPage
import pages.navigation.WebHeader
import pages.withNormalisedText
import utils.SerenityHelpers
import java.time.OffsetDateTime
import kotlin.math.truncate
import constants.TppConstants

open class V2MedicalRecordTestResultsStepDefinitions {

    private lateinit var testResultsPage: TestResultsPage
    private lateinit var myRecordDetailedTestResultPage: MyRecordTestResultDetailPage
    private lateinit var errorPage: ErrorPage
    private lateinit var chooseTestResultPage: MyRecordChooseTestResultYearPage
    private lateinit var webHeader: WebHeader

    private val pageCount = TppConstants.PageCount

    @Steps
    lateinit var browser: BrowserSteps

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

    @Then("I see the previous 5 years")
    fun thenISeeThePreviousFiveYears() {
        chooseTestResultPage.assertInitialTitleVisible()
        chooseTestResultPage.assertFirstMenuElement()
        chooseTestResultPage.assertLastMenuElement()
    }

    @Then("I see further previous test results")
    fun thenISeeFurtherPreviousFiveYears() {
        chooseTestResultPage.assertNextTitleVisible()
        chooseTestResultPage.assertNextFirstMenuElement()
        chooseTestResultPage.assertNextLastMenuElement()
    }

    @When("^I retrieve the last choose test results page$")
    fun iRetrieveTheLastChooseTestResultsPage() {
        val currentYear = OffsetDateTime.now().year
        val patientYearOfBirth = SerenityHelpers.getPatient().age.getYearOfBirth()

        val age = currentYear - patientYearOfBirth.toDouble()
        val lastPage = truncate(age / pageCount) + 1

        val urlForPage = PageUrl.getRelativePagePath("choose test results") + "?page=$lastPage"
        browser.browseTo(urlForPage)
    }

    @When("^I retrieve the last choose test results page for a linked account user$")
    fun iRetrieveTheLastChooseTestResultsPageForALinkedAccountUser() {
        val currentYear = OffsetDateTime.now().year
        val patientLinkedAccountYearOfBirth =
                SerenityHelpers.getPatient().linkedAccounts.toList()[0].age.getYearOfBirth()

        val age = currentYear - patientLinkedAccountYearOfBirth.toDouble()
        val lastPage = truncate(age / pageCount) + 1

        val urlForPage = PageUrl.getRelativePagePath("choose test results") + "?page=$lastPage"
        browser.browseTo(urlForPage)
    }

    @Then("^I see the specific test results page for '(.*)'$")
    fun iSeeTheSpecificTestResultsPage(year: String) {
        val title = "$year test results"
        webHeader.getPageTitle().withNormalisedText(title).assertIsVisible()
    }

    @When("^I retrieve the last specific test results page$")
    fun iRetrieveTheLastSpecificTestResultsPage() {
        val patientYearOfBirth = SerenityHelpers.getPatient().age.getYearOfBirth()

        val urlForPage = PageUrl.getRelativePagePath("test results for year") + "?year=$patientYearOfBirth"
        browser.browseTo(urlForPage)
    }

    @When("^I retrieve the last specific test results page for a linked account user$")
    fun iRetrieveTheLastSpecificTestResultsPageForALinkedAccountUser() {
        val patientLinkedAccountYearOfBirth =
                SerenityHelpers.getPatient().linkedAccounts.toList()[0].age.getYearOfBirth()


        val urlForPage = PageUrl.getRelativePagePath("test results for year") + "?year=$patientLinkedAccountYearOfBirth"
        browser.browseTo(urlForPage)
    }

    @When("^I click the first test result$")
    fun iClickTheFirstTestResults() {
        testResultsPage.clickTestResult()
    }

    @Then("^I click the previous years test results$")
    fun iClickThePreviousYearsTestResults() {
        val previousYear = OffsetDateTime.now().year - 1
        chooseTestResultPage.getHeaderElement(previousYear).click()
    }

    @Then("^I cannot see the next pagination link$")
    fun iCannotSeeTheNextPaginationLink() {
        chooseTestResultPage.assertNextPaginationNotVisible()
    }

    @Then("^I can see the next pagination link$")
    fun iCanSeeTheNextPaginationLink() {
        chooseTestResultPage.assertNextPaginationVisible()
    }

    @When("^I click the next pagination link$")
    fun iClickTheNextPaginationLink() {
        chooseTestResultPage.clickNextPaginationLink()
    }

    @Then("^I cannot see the previous pagination link$")
    fun iCannotSeeThePreviousPaginationLink() {
        chooseTestResultPage.assertPreviousPaginationNotVisible()
    }

    @Then("^I can see the previous pagination link$")
    fun iCanSeeThePreviousPaginationLink() {
        chooseTestResultPage.assertPreviousPaginationVisible()
    }

    @When("^I click the previous pagination link$")
    fun iClickThePreviousPaginationLink() {
        chooseTestResultPage.clickPreviousPaginationLink()
    }

    companion object {
        private const val emisDefaultTestResultsCount = 3
        private const val tppDefaultTestResultsCount = 6
    }
}
