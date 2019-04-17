package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.assertIsVisible
import pages.throttling.GPSearchResultsPage

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

    @Then("^the GP Practice found matches the searched postcode$")
    fun theGPPracticeFoundMatchesThePostcode() {
        gpSearchResultsPage.foundGPPracticeByPostcode.assertIsVisible()
    }

    @Then("^I see the GP Search Results Page with (\\d+) search results$")
    fun iSeeTheGPSearchResultsPage(numResults: Int) {
        gpSearchResultsPage.assertNumberOfResults(numResults)
    }

    @Then("^I see the GP Search Results Page with no search results$")
    fun iSeeTheGPSearchResultsPageWithNoResults() {
        gpSearchResultsPage.assertNoResults()
    }

    @Then("^the Too Many Results message for GP Search (is|is not) visible$")
    fun theTooManyResultsMessageIsOrIsNotVisible(isOrIsNot: String) {
        gpSearchResultsPage.tooManyResultsErrorHeaderIsVisible(isOrIsNot == "is")
    }

    @Then("^the Technical Problems error message for GP Search is visible$")
    fun theTechnicalProblemsErrorMessageIsOrIsNotVisible() {
        gpSearchResultsPage.assertTechnicalProblemsBanner()
    }

    @Then("^the No Results Found page for GP Search is visible$")
    fun theNoResultsFoundPageIsNotVisible() {
        gpSearchResultsPage.noResultsFoundErrorHeaderIsVisible()
    }
}

