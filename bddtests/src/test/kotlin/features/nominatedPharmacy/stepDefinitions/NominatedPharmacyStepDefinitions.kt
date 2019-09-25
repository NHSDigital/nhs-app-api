package features.nominatedPharmacy.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.nominatedPharmacy.steps.NominatedPharmacyDataSetupSteps
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import models.nominatedPharmacy.Postcode
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.isVisible
import pages.nominatedPharmacy.ConfirmNominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyResultsPage
import pages.nominatedPharmacy.SearchNominatedPharmacyPage
import pages.prescription.PrescriptionsPage
import pages.text
import utils.getOrFail
import utils.set

class NominatedPharmacyStepDefinitions {

    private lateinit var searchNominatedPharmacyPage: SearchNominatedPharmacyPage

    private lateinit var nominatedPharmacyPage: NominatedPharmacyPage

    private lateinit var confirmNominatedPharmacyPage: ConfirmNominatedPharmacyPage

    private lateinit var nominatedPharmacyResultsPage : NominatedPharmacyResultsPage

    private lateinit var prescriptionsPage: PrescriptionsPage

    @Steps
    private lateinit var nominatedPharmacyDataSetupSteps: NominatedPharmacyDataSetupSteps

    @Given("^searching for pharmacies with (.*) has (\\d+) results")
    fun searchTextHasResults(searchTerm: String, numberOfResults: Int) {

        val postcodeMatchesReply = NhsAzureSearchData.getSuccessfulPostcodeMatch()
        nominatedPharmacyDataSetupSteps.setupWiremockForPostcodeAndPlacesSearch(searchTerm, postcodeMatchesReply)

        val postcodeCoordinates = Postcode(
                latitude = postcodeMatchesReply.value[0].Latitude.toString(),
                longitude = postcodeMatchesReply.value[0].Longitude.toString()
        )

        val data = NhsAzureSearchData.generatePharmacyData(numberOfResults)
        NominatedPharmacySerenityHelpers.SEARCH_RESULTS.set(data)
        nominatedPharmacyDataSetupSteps.setupWiremockForPharmacyPostcodeSearch(postcodeCoordinates, data)
    }

    @Given("^my GP Practice is EPS enabled$")
    fun usersGPPracticeIsEPSEnabled() {
        nominatedPharmacyDataSetupSteps.enableGpPracticeForEPSForPatient()
    }

    @Given("^my GP Practice is EPS disabled$")
    fun usersGPPracticeIsEPSDisabled() {
        nominatedPharmacyDataSetupSteps.disableGpPracticeForEPSForPatient()
    }

    @Given("^I have a (.*) typed nominated pharmacy with (.*) OdsCode$")
    fun iHaveANominatedPharmacy(pharmacyType : String, odsCode: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacy(pharmacyType, odsCode)
    }

    @Given("^I have a P1 typed Internet pharmacy with (.*) OdsCode$")
    fun iHaveAnInternetPharmacy(odsCode: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacyWithInternetPharmacy(odsCode)
    }

    @Given("^I have a (.*) typed nominated pharmacy with (.*) OdsCode and nhsNumber (.*) is returned$")
    fun iHaveANomPharmButDifferentNhsNumberIsReturned(pharmacyType : String, odsCode: String, nhsNumber: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacyWithDifferentNhsNumber(pharmacyType, odsCode, nhsNumber)
    }

    @Given("^I have a (.*) typed nominated pharmacy with (.*) OdsCode and (.*) ConfidentialityCode$")
    fun iHaveANomPharmAndConfidentialityCode(pharmacyType : String, odsCode: String, confidentialityCode: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacy(pharmacyType, odsCode, confidentialityCode)
    }

    @Given("^I don't have a nominated pharmacy of any type$")
    fun iDontHaveANominatedPharmacyOfAnyType() {
        nominatedPharmacyDataSetupSteps.setupNoNominatedPharmacy()
    }

    @When("^I click on change my nominated pharmacy link$")
    fun iClickOnChangeMyNominatedPharmacyLink() {
        nominatedPharmacyPage.changePharmacyLink.click()
    }

    @When("^I click on the nominated pharmacy panel$")
    fun iClickTheNominatedPharmacyPanel() {
        prescriptionsPage.nominatedPharmacyPanel.click()
    }

    @When("^I search for a (.*) and click on search button$")
    fun iSearchForATextAndClickOnSearch(searchText: String) {
        searchNominatedPharmacyPage.enterSearchText(searchText)
        searchNominatedPharmacyPage.searchButton.click()
    }

    @When("^I click on item (\\d+) pharmacy from the list of pharmacies$")
    fun iClickOnAPharmacyFromTheListOfPharmacies(positionInTheList: Int) {
        val index = positionInTheList - 1
        nominatedPharmacyResultsPage.selectPharmacyAtIndex(index)

        val sessionData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        NominatedPharmacySerenityHelpers.PHARMACY_TO_BE_NOMINATED.set(sessionData[index])
    }

    @When("^I click on confirm button to change my nominated pharmacy$")
    fun iClickOnConfirmButton() {
        val sessionData = NominatedPharmacySerenityHelpers
                .PHARMACY_TO_BE_NOMINATED
                .getOrFail<NhsAzureSearchOrganisationItem>()
        nominatedPharmacyDataSetupSteps.setupWiremockForNominatedPharmacyPostUpdate("P1", sessionData)
        confirmNominatedPharmacyPage.confirmButton.click()
    }

    @Then("^I see the nominated pharmacy panel on the prescriptions page$")
    fun iSeeTheNominatedPharmacyBanner() {
        assertTrue(
                "Nominated pharmacy panel is visible", prescriptionsPage.nominatedPharmacyPanel.isVisible)
    }

    @Then("^I see my nominated pharmacy on the prescriptions page$")
    fun iSeeMyNominatedPharmacyOnThePrescriptionsPage() {
        val updatedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        assertEquals(
                "Nominated pharmacy name is not correct",
                updatedPharmacy.OrganisationName, prescriptionsPage.getNominatedPharmacyName())
    }

    @Then("^I see that I haven't nominated a pharmacy on the prescriptions page$")
    fun iSeeThatIHaventNominatedAPharmacyOnThePrescriptionsPage() {
        assertEquals("You've not nominated a pharmacy", prescriptionsPage.getNominatedPharmacyName())
    }

    @Then("^I see nominated pharmacy page loaded$")
    fun iAmRedirectedToNominatedPharmacyPage() {
        nominatedPharmacyPage.isLoadedWithPharmacy()
    }

    @Then("^I see nominated pharmacy page loaded with dispensing practise header$")
    fun iAmRedirectedToNominatedPharmacyPageShowingDispensingPractise() {
        nominatedPharmacyPage.isLoadedWithDispensingPractiseHeader()
    }

    @Then("^I see nominated pharmacy page loaded without a pharmacy$")
    fun iSeeNominatedPharmacyPageLoadedWithoutAPharmacy() {
        nominatedPharmacyPage.isLoadedWithNoPharmacy()
    }

    @Then("^I see the change my nominated pharmacy link$")
    fun iSeeChangePharmacyLink() {
        assertTrue(
                "Change my nominated pharmacy link is not visible",
                nominatedPharmacyPage.changePharmacyLink.isVisible)
    }

    @Then("^I see search nominated pharmacy page loaded$")
    fun iSeeSearchNominatedPharmacyLoaded() {
        searchNominatedPharmacyPage.isLoaded()
    }

    @Then("^I see the no results found page$")
    fun iSeeNoResultsFound() {
        nominatedPharmacyResultsPage.showsNoResultsFoundHeader()
    }

    @Then("^I see list of pharmacies displayed on the result page$")
    fun iSeePharmaciesOnResultsPage(){
        nominatedPharmacyResultsPage.isLoaded()
        val expectedData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        val searchResults = nominatedPharmacyResultsPage.getPharmacies()

        expectedData.forEachIndexed {
            index, dataItem ->
            assertEquals(
                    "Pharmacy name is not correct",
                    dataItem.OrganisationName, searchResults[index].pharmacyName)
            assertEquals(
                    "Pharmacy address is not correct",
                    dataItem.addressFormatted(), searchResults[index].address)

            val phoneNumber = dataItem.primaryPhone()
            if (phoneNumber != null) {
                assertEquals(
                        "Phone number is not correct",
                        phoneNumber, searchResults[index].phoneNumber)
            }
        }
    }

    @Then("^I see confirm nominated page with selected pharmacy details$")
    fun iSeeConfirmNominatedPharmacyPage() {
        confirmNominatedPharmacyPage.isLoaded()
        val selectedPharmacy = NominatedPharmacySerenityHelpers
                .PHARMACY_TO_BE_NOMINATED
                .getOrFail<NhsAzureSearchOrganisationItem>()
        assertEquals(
                "Organisation name is not correct",
                selectedPharmacy.OrganisationName, confirmNominatedPharmacyPage.pharmacyName.text)
        assertEquals(
                "Address is not correct",
                selectedPharmacy.addressFormatted(), confirmNominatedPharmacyPage.pharmacyAddress.text)
        val phoneNumber = selectedPharmacy.primaryPhone()
        if (phoneNumber != null) {
            assertEquals(
                    "Phone number is not correct",
                    phoneNumber, confirmNominatedPharmacyPage.pharmacyPhoneNumber.text)
        }
    }

    @Then("^I see my nominated pharmacy page with updated pharmacy details$")
    fun iSeeNominatedPharmacyPageWithUpdatedPharmacyDetails() {
        nominatedPharmacyPage.isLoadedWithPharmacy()
        nominatedPharmacyPage.assertYouHaveChangedYourPharmacySuccessBannerIsVisible()

        checkPharmacyDetailsAreCorrect()
    }

    @Then("^I see my nominated pharmacy page with chosen pharmacy details$")
    fun iSeeNominatedPharmacyPageWithChosenPharmacyDetails() {
        nominatedPharmacyPage.isLoadedWithPharmacy()
        nominatedPharmacyPage.assertYouHaveChosenYourNominatedPharmacyBannerIsVisible()

        checkPharmacyDetailsAreCorrect()
    }

    @Then("^I see how to change dispensing practice instruction$")
    fun iSeeHowToChangeDispensingPractise() {
        assertTrue("Instruction 1 to change pharmacy is not visible",
                nominatedPharmacyPage.cannotChangeDispensingPractiseInformationLine1.isVisible)

        assertTrue("Instruction 2 to change pharmacy is not visible",
                nominatedPharmacyPage.cannotChangeDispensingPractiseInformationLine2.isVisible)
    }

    private fun checkPharmacyDetailsAreCorrect() {
        val selectedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        assertEquals(
                "Pharmacy name is not correct",
                selectedPharmacy.OrganisationName, nominatedPharmacyPage.pharmacyName.text)
        assertEquals(
                "Pharmacy address is not correct",
                selectedPharmacy.addressFormatted(), nominatedPharmacyPage.pharmacyAddress.text)
    }
}
