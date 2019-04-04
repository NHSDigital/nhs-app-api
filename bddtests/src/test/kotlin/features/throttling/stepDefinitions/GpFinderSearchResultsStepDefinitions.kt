package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import org.junit.Assert.assertTrue
import pages.assertIsVisible
import pages.throttling.GPSearchResultsPage

private const val TECHNICAL_PROBLEMS = "Technical problems"
private const val TOO_MANY_RESULTS = "Too many results"
private const val NO_RESULTS_FOUND = "No results found"
private const val NO_RESULTS_COUNT = 0
private const val MAX_ORGANISATION_RESULTS_COUNT = NhsAzureSearchData.ORGANISATION_LIMIT
private const val SOME_RESULTS_COUNT = 2
private const val POSTCODE_SEARCH_RESULTS_COUNT = 1

open class GpFinderSearchResultsStepDefinitions {

    lateinit var gpSearchResultsPage: GPSearchResultsPage

    @When("^I select a practice which is participating in beta$")
    fun iSelectAPracticeWhichIsParticipatingInBeta() {
        gpSearchResultsPage.participatingGPPractice.click()
    }

    @When("^I select a practice which is not participating in beta$")
    fun iSelectAPracticeWhichIsNotParticipatingInBeta() {
        gpSearchResultsPage.notParticipatingGPPractice.click()
    }

    @Then("^The GP Practice found matches the searched postcode$")
    fun theGPPracticeFoundMatchesThePostcode() {
        gpSearchResultsPage.foundGPPracticeByPostcode.assertIsVisible()
    }

    @Then("^I see the GP Search Results Page with " +
            "($NO_RESULTS_COUNT|$SOME_RESULTS_COUNT|$MAX_ORGANISATION_RESULTS_COUNT|$POSTCODE_SEARCH_RESULTS_COUNT) " +
            "search results$")
    fun iSeeTheGPSearchResultsPage(numResults: String) {
        assertTrue(gpSearchResultsPage.resultsExistForSearch(numResults.toInt()))
    }

    @Then("^The ($TOO_MANY_RESULTS|$TECHNICAL_PROBLEMS|$NO_RESULTS_FOUND) error message (is|is not) visible$")
    fun theErrorMessageIsOrIsNotVisible(errorType: String, isOrIsNot: String) {
        when (errorType) {
            TECHNICAL_PROBLEMS -> {
                gpSearchResultsPage.technicalProblemsErrorHeaderIsVisible(isOrIsNot == "is")
            }
            TOO_MANY_RESULTS -> {
                gpSearchResultsPage.tooManyResultsErrorHeaderIsVisible(isOrIsNot == "is")
            }
            NO_RESULTS_FOUND -> {
                gpSearchResultsPage.noResultsFoundErrorHeaderIsVisible(isOrIsNot == "is")
            }
        }
    }
}

