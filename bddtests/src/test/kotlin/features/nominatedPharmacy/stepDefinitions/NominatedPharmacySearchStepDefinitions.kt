package features.nominatedPharmacy.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import pages.nominatedPharmacy.SearchNominatedPharmacyPage

class NominatedPharmacySearchStepDefinitions {

    private lateinit var searchNominatedPharmacyPage: SearchNominatedPharmacyPage

    @When("^I search for a (.*) and click on search button$")
    fun iSearchForATextAndClickOnSearch(searchText: String) {
        searchNominatedPharmacyPage.enterSearchText(searchText)
        searchNominatedPharmacyPage.searchButton.click()
    }

    @Then("^I see an error indicating the postcode is invalid$")
    fun iSeeThePostCodeIsInvalidErrorMessage() {
        Assert.assertTrue(searchNominatedPharmacyPage.isInvalidPostcodeErrorVisible())
    }

    @Then("^I see the no results found messages for (.*)$")
    fun iSeeNoResultsFoundMessage(searchText: String) {
        Assert.assertTrue(searchNominatedPharmacyPage.isNoResultsFoundHeaderVisible(searchText))
        Assert.assertTrue(searchNominatedPharmacyPage.isNoResultsFoundMessageVisible(searchText))
        Assert.assertTrue(searchNominatedPharmacyPage.isSearchAgainVisible())
    }

    @Then("^I see search nominated pharmacy page loaded$")
    fun iSeeSearchNominatedPharmacyLoaded() {
        searchNominatedPharmacyPage.isLoaded()
    }
}
