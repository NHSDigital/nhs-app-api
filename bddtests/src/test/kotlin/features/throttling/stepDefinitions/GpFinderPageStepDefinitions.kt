package features.throttling.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertTrue
import pages.throttling.GPFinderPage
import utils.getOrFail
import utils.set

private const val NO = "no"
private const val BLANK = "blank"

open class GpFinderPageStepDefinitions {

    private val mockingClient = MockingClient.instance

    lateinit var gpFinderPage: GPFinderPage

    @Steps
    lateinit var browser: BrowserSteps

    @Given("I am not logged in and I have not completed the beta throttling flow$")
    open fun iHaveNotLoggedInAndIHaveNotPreviouslySelectedMyGPPractice() {
        browser.goToApp()
        gpFinderPage.driver.manage().deleteAllCookies()
        browser.goToApp()
        //browser.appendSourceQueryString("ios")
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.jackJackson)
    }

    @When("^I enter criteria and submit my search in the GP Practice finder")
    fun iSubmitMySearch() {
        val searchText= ThrottlingSerenityHelpers.SEARCH_TEXT.getOrFail<String>()
        gpFinderPage.enterSearchTerm(searchText)
        gpFinderPage.clickContinueButton()
    }

    @When("^I submit ($NO|$BLANK) search criteria$")
    fun iSubmitInvalidSearchCriteria(noOrBlank: String) {
        when (noOrBlank) {
            NO -> {
                ThrottlingSerenityHelpers.SEARCH_TEXT.set(GPFinderPage.emptyInvalidSearch)
            }
            BLANK -> {
                ThrottlingSerenityHelpers.SEARCH_TEXT.set(GPFinderPage.blankInvalidSearch)
            }
        }
        iSubmitMySearch()
    }

    @When("^I click the link to skip the throttling flow$")
    fun iClickTheLinkToSkipTheThrottlingFlow() {
        gpFinderPage.clickLoginButton()
    }

    @Then("^I see the GP Finder Page$")
    fun assertGPFinderPageVisible() {
        assertTrue(gpFinderPage.isFindYourGPSurgeryHeaderVisible())
    }

    @Then("^I see the GP Finder page with a search criteria error message$")
    fun iSeeTheGPFinderPageWithASearchCriteriaErrorMessage() {
        assertTrue(gpFinderPage.isSearchCriteriaErrorMessageShown())
    }
}

