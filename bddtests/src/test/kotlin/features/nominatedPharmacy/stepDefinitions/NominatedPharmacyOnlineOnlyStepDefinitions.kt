package features.nominatedPharmacy.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.nominatedPharmacy.steps.NominatedPharmacyDataSetupSteps
import features.nominatedPharmacy.steps.NominatedPharmacyOnlinePharmacyDataSetupSteps
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.nominatedPharmacy.ConfirmOnlineNominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyChooseTypePage
import pages.nominatedPharmacy.NominatedPharmacyDspInterruptPage
import pages.nominatedPharmacy.NominatedPharmacyOnlineOnlyChoicesPage
import pages.nominatedPharmacy.NominatedPharmacyOnlineOnlySearchPage
import pages.nominatedPharmacy.NominatedPharmacyResultsPage
import pages.nominatedPharmacy.OnlineNominatedPharmacyChangeSuccessPage
import pages.text
import utils.getOrFail
import utils.set

class NominatedPharmacyOnlineOnlyStepDefinitions {

    private lateinit var nominatedPharmacyResultsPage : NominatedPharmacyResultsPage

    private lateinit var nominatedPharmacyOnlineOnlySearchPage: NominatedPharmacyOnlineOnlySearchPage

    private lateinit var nominatedPharmacyChooseTypePage: NominatedPharmacyChooseTypePage

    private lateinit var nominatedPharmacyOnlineOnlyChoicesPage: NominatedPharmacyOnlineOnlyChoicesPage

    private lateinit var nominatedPharmacyOnlineConfirmPage: ConfirmOnlineNominatedPharmacyPage

    private lateinit var nominatedPharmacyOnlineOnlyChangeSuccessPage: OnlineNominatedPharmacyChangeSuccessPage

    private lateinit var nominatedPharmacyDspInterruptPage: NominatedPharmacyDspInterruptPage

    @Steps
    private lateinit var nominatedPharmacyOnlinePharmacyDataSetupSteps: NominatedPharmacyOnlinePharmacyDataSetupSteps

    @Steps
    private lateinit var nominatedPharmacyDataSetupSteps: NominatedPharmacyDataSetupSteps


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

    @When("^I search for an online only pharmacy with (.*) and click on search button$")
    fun iSearchForAOnlineOnlyPharmacyWithTextAndClickOnSearch(searchText: String) {
        nominatedPharmacyOnlineOnlySearchPage.enterTextToSearchField(searchText)
        nominatedPharmacyOnlineOnlySearchPage.searchButton.click()
    }

    @When("^I search for an online only pharmacy using dangerous text and click on search button$")
    fun iSearchForAOnlineOnlyPharmacyWithDangerousTextAndClickOnSearch() {
        nominatedPharmacyOnlineOnlySearchPage.enterTextToSearchField("<script>")
        nominatedPharmacyOnlineOnlySearchPage.searchButton.click()
    }

    @When("^I click on confirm button to change my nominated pharmacy to online$")
    fun iClickOnConfirmButton() {
        val sessionData = NominatedPharmacySerenityHelpers
                .PHARMACY_TO_BE_NOMINATED
                .getOrFail<NhsAzureSearchOrganisationItem>()
        nominatedPharmacyDataSetupSteps.setupWiremockForNominatedPharmacyPostUpdate("P1", sessionData)
        nominatedPharmacyOnlineConfirmPage.confirmButton.click()
    }

    @When("^I click on the DSP Interrupt Prescription Home link$")
    fun iClickTheDspInterruptPrescriptionHomeLink() {
        nominatedPharmacyDspInterruptPage.prescriptionsHomeLink.click()
    }

    @When("^I click on item (\\d+) pharmacy from the list of online pharmacies$")
    fun iClickOnAPharmacyFromTheListOfOnlinePharmacies(positionInTheList: Int) {
        val index = positionInTheList - 1

        val clickedOnlinePharmacy = nominatedPharmacyResultsPage.getOnlinePharmacies()[index]
        // Finds all of the search results that are displayed on screen
        val sessionData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        // Does a comparison on the sessionData and populates the SerenityHelper with the correct
        // pharmacy result based on pharmacyName matching
        val result = (sessionData.find { it.OrganisationName.equals(clickedOnlinePharmacy.pharmacyName)})
        if (result != null) {
            NominatedPharmacySerenityHelpers.PHARMACY_TO_BE_NOMINATED.set(result)
        } else {
            Assert.fail("Selected pharmacy not found in the session data")
        }
        //Performs click action on the pharmacy result to navigate to next page in flow
        nominatedPharmacyResultsPage.selectPharmacyAtIndex(index)
    }

    @Then("^I see confirm nominated page with selected online pharmacy details$")
    fun iSeeConfirmNominatedPharmacyPageWithSelectedOnlinePharmacyDetails() {
        nominatedPharmacyOnlineConfirmPage.isLoaded()
        val selectedPharmacy = NominatedPharmacySerenityHelpers
                .PHARMACY_TO_BE_NOMINATED
                .getOrFail<NhsAzureSearchOrganisationItem>()
        Assert.assertEquals(
                "Organisation name is not correct",
                selectedPharmacy.OrganisationName, nominatedPharmacyOnlineConfirmPage.pharmacyName.text)
        if(selectedPharmacy.URL != null) {
            Assert.assertEquals(
                    "Url is not correct",
                    selectedPharmacy.URL, nominatedPharmacyOnlineConfirmPage.pharmacyUrl.text)
        }
        val phoneNumber = selectedPharmacy.primaryPhone()
        if (phoneNumber != null) {
            Assert.assertEquals(
                    "Phone number is not correct",
                    "Telephone: $phoneNumber", nominatedPharmacyOnlineConfirmPage.pharmacyPhoneNumber.text)
        }
    }

    @Then("^I see the change success page with my online nominated pharmacy details$")
    fun iSeeTheChangeSuccessPage() {

        val myNominatedPharmacy =
                NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.getOrFail<NhsAzureSearchOrganisationItem>()

        nominatedPharmacyOnlineOnlyChangeSuccessPage.isLoaded()

        Assert.assertEquals(
                "Organisation name is not correct",
                myNominatedPharmacy.OrganisationName, nominatedPharmacyOnlineOnlyChangeSuccessPage.pharmacyName.text)

        if(myNominatedPharmacy.URL != null){
            Assert.assertEquals(
                    "Url is not correct",
                    myNominatedPharmacy.URL, nominatedPharmacyOnlineOnlyChangeSuccessPage.pharmacyUrl.text)
        }

        val phoneNumber = myNominatedPharmacy.primaryPhone()

        if (phoneNumber != null) {
            Assert.assertEquals(
                    "Phone number is not correct",
                    "Telephone: $phoneNumber", nominatedPharmacyOnlineOnlyChangeSuccessPage.pharmacyPhoneNumber.text)
        }
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

    @Then("^I see an error indicating the search text is invalid$")
    fun iSeeAnErrorIndicatingTheSearchTextIsInvalid() {
        Assert.assertTrue(nominatedPharmacyOnlineOnlySearchPage.isErrorMessageTextVisible())
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

    @Then("^I see list of online only pharmacies displayed on the result page$")
    fun iSeeOnlineOnlyPharmaciesOnResultsPage(){
        nominatedPharmacyResultsPage.assertIsLoaded()
        val expectedData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        val searchResults = nominatedPharmacyResultsPage.getOnlinePharmacies()

        expectedData.forEachIndexed {
            index, dataItem ->
            Assert.assertEquals(
                    "Online Pharmacy name is not correct",
                    dataItem.OrganisationName, searchResults[index].pharmacyName)
            if(dataItem.URL != null){
                Assert.assertEquals(
                        "Online Pharmacy URL is not correct",
                        dataItem.URL, searchResults[index].website)
            }

            val phoneNumberData = dataItem.primaryPhone()
            val phoneNumber = "Telephone: $phoneNumberData"
            if (phoneNumberData != null) {
                Assert.assertEquals(
                        "Online Pharmacy Phone number is not correct",
                        phoneNumber, searchResults[index].phoneNumber)
            }
        }
    }

    @Then("^I click on the choose type continue button$")
    fun iClickOnTheChooseTypeContinueButton() {
        nominatedPharmacyChooseTypePage.continueButton.click()
    }

    @Then("^I see the dsp interrupt page is loaded$")
    fun iSeeDspInterruptPageIsLoaded() {
        nominatedPharmacyDspInterruptPage.isLoaded()
    }

    @Then("^I see list of random online pharmacies displayed on the result page$")
    fun iSeeOnlinePharmaciesOnResultsPage(){
        nominatedPharmacyResultsPage.assertIsLoaded()
        val searchResults = nominatedPharmacyResultsPage.getOnlinePharmacies()
        Assert.assertEquals(DEFAULT_SEARCH_RESULT_COUNT, searchResults.size)
    }

    companion object {
        const val DEFAULT_SEARCH_RESULT_COUNT = 10
    }
}
