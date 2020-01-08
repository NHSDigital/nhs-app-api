package features.nominatedPharmacy.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.nominatedPharmacy.steps.NominatedPharmacyOnlinePharmacyDataSetupSteps
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.nominatedPharmacy.NominatedPharmacyChooseTypePage
import pages.nominatedPharmacy.NominatedPharmacyResultsPage
import pages.nominatedPharmacy.NominatedPharmacyOnlineOnlyChoicesPage
import pages.nominatedPharmacy.NominatedPharmacyOnlineOnlySearchPage
import utils.set

class NominatedPharmacyOnlineOnlyStepDefinitions {

    private lateinit var nominatedPharmacyResultsPage : NominatedPharmacyResultsPage

    private lateinit var nominatedPharmacyOnlineOnlySearchPage: NominatedPharmacyOnlineOnlySearchPage

    private lateinit var nominatedPharmacyChooseTypePage: NominatedPharmacyChooseTypePage

    private lateinit var nominatedPharmacyOnlineOnlyChoicesPage: NominatedPharmacyOnlineOnlyChoicesPage

    @Steps
    private lateinit var nominatedPharmacyOnlinePharmacyDataSetupSteps: NominatedPharmacyOnlinePharmacyDataSetupSteps


    @Given("^searching for online pharmacies with (.*) has (\\d+) results")
    fun searchOnlineOnlyTextHasResults(searchTerm: String, numberOfItems: Int) {
        val data = NhsAzureSearchData.generateOnlinePharmacySearchData(numberOfItems)
        nominatedPharmacyOnlinePharmacyDataSetupSteps
                .setupWiremockForSearchedOnlinePharmaciesWithSearch(data, searchTerm)

        NominatedPharmacySerenityHelpers.SEARCH_RESULTS.set(data)
    }

    @Given("^searching for randomized online pharmacies has results")
    fun randomisedOnlinePharmacyResults() {
        val randomInternetPharmacies = NhsAzureSearchData.generateOnlinePharmacyData()
        nominatedPharmacyOnlinePharmacyDataSetupSteps
                .setupWiremockForRandomisedOnlinePharmacies(randomInternetPharmacies)

        NominatedPharmacySerenityHelpers.SEARCH_RESULTS.set(randomInternetPharmacies)
    }

    @When("^I search for an online only pharmacy with postcode (.*) and click on search button$")
    fun iSearchForAOnlineOnlyPharmacyWithPostcodeAndClickOnSearch(searchText: String) {
        nominatedPharmacyOnlineOnlySearchPage.enterTextToSearchField(searchText)
        nominatedPharmacyOnlineOnlySearchPage.searchButton.click()
    }

    @Then("^I see nominated pharmacy online only search page loaded$")
    fun iSeeNominatedPharmacyOnlineOnlySearchPage() {
        nominatedPharmacyOnlineOnlySearchPage.isLoaded()
    }

    @Then("^I see the online choices page loaded$")
    fun iSeeTheOnlineChoicesLoaded() {
        nominatedPharmacyOnlineOnlyChoicesPage.isLoaded()
    }

    @Then("^I do not see any online only pharmacies$")
    fun iDoNotSeeTheOnlinePharmaciesLoaded() {
        nominatedPharmacyResultsPage.showsNoResults()
    }

    @Then("^I see the relevant information about no results for the search term (.*)$")
    fun iSeeTheRelevantInformationAboutNoResults(searchTerm: String) {
        nominatedPharmacyOnlineOnlySearchPage.isNoResultsFoundHeaderVisible(searchTerm)
        nominatedPharmacyOnlineOnlySearchPage.isNoResultsFoundMessageVisible(searchTerm)
        nominatedPharmacyOnlineOnlySearchPage.isSearchAgainH2Visible()
    }


    @Then("^I click the No radio button on the online choices page$")
    fun iClickTheNoRadioButtonOnTheOnlineChoicesPage() {
        nominatedPharmacyOnlineOnlyChoicesPage.noRadioButton.click()
    }

    @Then("^I click the Yes radio button on the online choices page$")
    fun iClickTheYesRadioButtonOnTheOnlineChoicesPage() {
        nominatedPharmacyOnlineOnlyChoicesPage.yesRadioButton.click()
    }

    @Then("^I click on the continue button on the online choices page$")
    fun iClickTheContinueButtonOnTheOnlineChoicesPage() {
        nominatedPharmacyOnlineOnlyChoicesPage.continueButton.click()
    }

    @Then("^I see the choose type page is loaded$")
    fun iSeeChooseTypePageIsLoaded() {
        nominatedPharmacyChooseTypePage.isLoaded()
    }

    @When("^I select online pharmacy$")
    fun iSelectOnlinePharmacy() {
        nominatedPharmacyChooseTypePage.onlinePharmacyRadioButton.click()
    }

    @Then("^I click on the choose type continue button$")
    fun iClickOnTheChooseTypeContinueButton() {
        nominatedPharmacyChooseTypePage.continueButton.click()
    }

    @Then("^I see list of random online pharmacies displayed on the result page$")
    fun iSeeOnlinePharmaciesOnResultsPage(){
        nominatedPharmacyResultsPage.isLoaded()
        val searchResults = nominatedPharmacyResultsPage.getOnlinePharmacies()
        Assert.assertEquals(DEFAULT_SEARCH_RESULT_COUNT, searchResults.size)
    }

    companion object {
        const val DEFAULT_SEARCH_RESULT_COUNT = 10
    }
}
