package features.throttling.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.nhsAzureSearchService.NhsAzureSearchRequestBody
import models.Patient
import net.thucydides.core.annotations.NotImplementedException
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertTrue
import pages.CIDAccountCreationPage
import pages.throttling.GPFinderPage
import pages.throttling.GPParticipationPage
import pages.throttling.GPSearchResultsPage

private const val IS = "is"
private const val IS_NOT = "is not"
private const val TECHNICAL_PROBLEMS = "Technical problems"
private const val TOO_MANY_RESULTS = "Too many results"
private const val NO_RESULTS_FOUND = "No results found"
private const val NO_RESULTS_COUNT = 0
private const val MAX_RESULTS_COUNT = NhsAzureSearchData.LIMIT
private const val SOME_RESULTS_COUNT = 2
private const val NO = "no"
private const val BLANK = "blank"
private const val MULTIPLE = "multiple"
private const val MAXIMUM_LIMIT = "the maximum limit"
private const val MORE_THAN_MAXIMUM = "more than the maximum"

open class ThrottlingStepDefinitions {

    private val mockingClient = MockingClient.instance
    private var searchText = ""

    @Steps
    private lateinit var browser: BrowserSteps
    private lateinit var gpFinderPage: GPFinderPage
    private lateinit var gpSearchResultsPage: GPSearchResultsPage
    private lateinit var gpParticipationPage: GPParticipationPage
    private lateinit var cidAccountCreationPage: CIDAccountCreationPage

    @Given("^I see the GP Finder Page$")
    fun assertGPFinderPageVisible() {
        Assert.assertTrue(gpFinderPage.isFindYourGPSurgeryHeaderVisible())
    }

    @Given("^I see the GP Search Results Page with (\\d+) search results$")
    fun iSeeTheGPSearchResultsPage(numResults: String) {
        when (numResults) {
            "$NO_RESULTS_COUNT" -> {
                assertTrue(gpSearchResultsPage.testResultsExistForSearch(NO_RESULTS_COUNT))
            }
            "$SOME_RESULTS_COUNT" -> {
                assertTrue(gpSearchResultsPage.testResultsExistForSearch(SOME_RESULTS_COUNT))
            }
            "$MAX_RESULTS_COUNT" -> {
                assertTrue(gpSearchResultsPage.testResultsExistForSearch(MAX_RESULTS_COUNT))
            }
            else -> {
                throw NotImplementedException("Checking for unexpected number of results")
            }
        }
    }

    @Given("^There are ($MULTIPLE|$NO|$MAXIMUM_LIMIT|$MORE_THAN_MAXIMUM) GP Practices for my search criteria$")
    fun thereAreXGPPracticesForMySearchCriteria(howManyPractices: String) {
        when(howManyPractices) {
            MULTIPLE -> {
                mockingClient.forNhsAzureSearch {
                    nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                            search = "${GPFinderPage.validSearch}*"))
                            .respondWithSuccess(NhsAzureSearchData.getLessThanMaxNumberOfSearchData())
                }
            }
            NO -> {
                mockingClient.forNhsAzureSearch {
                    nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                            search = "${GPFinderPage.validSearch}*"))
                            .respondWithSuccess(NhsAzureSearchData.getZeroSearchData())
                }
            }
            MAXIMUM_LIMIT -> {
                mockingClient.forNhsAzureSearch {
                    nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                            search = "${GPFinderPage.validSearch}*"))
                            .respondWithSuccess(NhsAzureSearchData.getMaxNumberOfSearchData())
                }
            }
            MORE_THAN_MAXIMUM -> {
                mockingClient.forNhsAzureSearch {
                    nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                            search = "${GPFinderPage.validSearch}*"))
                            .respondWithSuccess(NhsAzureSearchData.getMoreThanMaxNumberOfSearchData())
                }
            }
        }
        searchText = GPFinderPage.validSearch
    }

    @Given("^The NHS Service Search is unavailable$")
    fun iSearchForAGPPracticeWhenTheNHSServiceSearchIsUnavailable() {
        mockingClient.forNhsAzureSearch {
            nhsAzureSearch.nhsAzureSearchRequest(NhsAzureSearchRequestBody(
                    search = "${GPFinderPage.validSearch}*"))
                    .respondWithServiceUnavailable()
        }
        searchText = GPFinderPage.validSearch
    }

    @Given("^I submit my search")
    fun iSubmitMySearch() {
        gpFinderPage.enterSearchTerm(searchText)
        gpFinderPage.clickContinueButton()
    }

    @Given("^I submit ($NO|$BLANK) search criteria$")
    fun iSubmitInvalidSearchCriteria(noOrBlank: String) {
        when(noOrBlank) {
            NO -> {
                searchText = GPFinderPage.emptyInvalidSearch
            }
            BLANK -> {
                searchText = GPFinderPage.blankInvalidSearch
            }
        }
        iSubmitMySearch()
    }

    @Given("I am not logged in and I have not completed the beta throttling flow$")
    open fun iHaveNotLoggedInAndIHaveNotPreviouslySelectedMyGPPractice() {
        browser.goToApp()
        gpFinderPage.driver.manage().deleteAllCookies()
        browser.goToApp()
    }

    @Given("^I have searched for my GP Practice$")
    fun iHaveSearchedForMyGPPractice() {
        assertGPFinderPageVisible()
        thereAreXGPPracticesForMySearchCriteria(MULTIPLE)
        iSeeTheGPSearchResultsPage("$SOME_RESULTS_COUNT")
    }

    @When("^My GP Practice ($IS|$IS_NOT) participating in beta$")
    fun myGPPracticeIsOrIsNotParticipatingInBeta(isOrIsNot: String) {
        val participating = isOrIsNot == IS
        gpSearchResultsPage.setPracticeToSelect(participating)
        gpParticipationPage.setHeaderToLookFor(participating)
    }

    @When("^I select my GP Practice$")
    fun iSelectMyGPPractice() {
        gpSearchResultsPage.selectMyGpPractice()
    }

    @When("^I see the Practice (Participating|Not Participating) page$")
    fun iSeeThePracticeParticipatingOrNotParticipatingPage(participating: String) {
        gpParticipationPage.featuresUsedHeader.assertIsVisible()
        gpParticipationPage.currentlyAvailableHeader.assertIsVisible()
        gpParticipationPage

        when(participating) {
            "Participating" -> {
                gpParticipationPage.assertParticipatingFeaturesVisible()
                gpParticipationPage.createAccountMessage.assertIsVisible()
                gpParticipationPage.ctaCreateAccountButton.assertIsVisible()
                gpParticipationPage.limitingFeaturesWarning.assertIsVisible()
            }
            "Not Participating" -> {
                gpParticipationPage.assertNotParticipatingFeaturesVisible()
                gpParticipationPage.comingSoonHeader.assertIsVisible()
                gpParticipationPage.ctaContinueButton.assertIsVisible()
            }
        }
    }

    @When("^I click the This is not my GP surgery button$")
    fun iClickTheThisIsNotMyGPSurgeryButton() {
        gpParticipationPage.clickNotMySurgeryButton()
    }

    @When("^I click the link to skip the throttling flow$")
    fun iClickTheLinkToSkipTheThrottlingFlow() {
        gpFinderPage.clickSkipThrottlingLink()
    }

    @When("^I click the Create Account button on the Practice Participating page$")
    fun whenIClickTheCreateAccountButtonOnThePracticeParticipatingPage() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.jackJackson)
        gpParticipationPage.ctaCreateAccountButton.click()
    }

    @Then("^The ($TOO_MANY_RESULTS|$TECHNICAL_PROBLEMS|$NO_RESULTS_FOUND) error message ($IS|$IS_NOT) visible$")
    fun theErrorMessageIsOrIsNotVisible(errorType: String, isOrIsNot: String) {
        when (errorType) {
            TECHNICAL_PROBLEMS -> {
                gpSearchResultsPage.technicalProblemsErrorHeaderIsVisible(isOrIsNot == IS)
            }
            TOO_MANY_RESULTS -> {
                gpSearchResultsPage.tooManyResultsErrorHeaderIsVisible(isOrIsNot == IS)
            }
            NO_RESULTS_FOUND -> {
                gpSearchResultsPage.noResultsFoundErrorHeaderIsVisible(isOrIsNot == IS)
            }
        }
    }

    @Then("^I see the GP Finder page with a search criteria error message$")
    fun iSeeTheGPFinderPageWithASearchCriteriaErrorMessage() {
        assertTrue(gpFinderPage.isSearchCriteriaErrorMessageShown())
    }

    @Then("^I see the CID login page$")
    fun iSeeTheCIDLoginPage() {
        assertTrue(cidAccountCreationPage.isVisible())
    }

}

