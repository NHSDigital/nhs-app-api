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
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.isVisible
import pages.nominatedPharmacy.NominatedPharmacyChooseTypePage
import pages.nominatedPharmacy.SearchNominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyPage
import pages.nominatedPharmacy.ConfirmNominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyResultsPage
import pages.nominatedPharmacy.NominatedPharmacyChangeSuccessPage
import pages.nominatedPharmacy.NominatedPharmacyInterruptPage
import pages.prescription.PrescriptionsPage
import pages.text
import utils.SerenityHelpers
import utils.getOrFail
import utils.set

class NominatedPharmacyStepDefinitions {

    private lateinit var searchNominatedPharmacyPage: SearchNominatedPharmacyPage

    private lateinit var nominatedPharmacyPage: NominatedPharmacyPage

    private lateinit var confirmNominatedPharmacyPage: ConfirmNominatedPharmacyPage

    private lateinit var nominatedPharmacyResultsPage : NominatedPharmacyResultsPage

    private lateinit var prescriptionsPage: PrescriptionsPage

    private lateinit var nominatedPharmacyChangeSuccessPage: NominatedPharmacyChangeSuccessPage

    private lateinit var nominatedPharmacyInterruptPage: NominatedPharmacyInterruptPage

    private lateinit var nominatedPharmacyChooseTypePage: NominatedPharmacyChooseTypePage

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

    @Given("^I have a (.*) typed nominated pharmacy$")
    fun iHaveANominatedPharmacy(pharmacyType : String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacy(pharmacyType, SerenityHelpers.getPatient().odsCode)
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

    @When("^I click on change your nominated pharmacy link$")
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

    @When("^I click on item (\\d+) pharmacy from the list of online pharmacies$")
    fun iClickOnAPharmacyFromTheListOfOnlinePharmacies(positionInTheList: Int) {
        val index = positionInTheList - 1

        val clickedOnlinePharmacy = nominatedPharmacyResultsPage.getOnlinePharmacies()[index]

        val sessionData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

            val result = (sessionData.find { it.OrganisationName.equals(clickedOnlinePharmacy.pharmacyName)})
            if (result != null) {
                NominatedPharmacySerenityHelpers.PHARMACY_TO_BE_NOMINATED.set(result)
            } else {
                Assert.fail("Selected pharmacy not found in the session data")
            }

        nominatedPharmacyResultsPage.selectPharmacyAtIndex(index)
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

    @Then("^I see the change success page with my nominated pharmacy details$")
    fun iSeeTheChangeSuccessPage() {

        val myNominatedPharmacy =
                NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.getOrFail<NhsAzureSearchOrganisationItem>()

        nominatedPharmacyChangeSuccessPage.isLoaded()

        assertEquals(
                "Organisation name is not correct",
                myNominatedPharmacy.OrganisationName, nominatedPharmacyChangeSuccessPage.pharmacyName.text)
        assertEquals(
                "Address is not correct",
                myNominatedPharmacy.addressFormatted(), nominatedPharmacyChangeSuccessPage.pharmacyAddress.text)

        val phoneNumber = myNominatedPharmacy.primaryPhone()

        if (phoneNumber != null) {
            assertEquals(
                    "Phone number is not correct",
                    "Telephone: " + phoneNumber, confirmNominatedPharmacyPage.pharmacyPhoneNumber.text)
        }
    }

    @When("^I click on the go to your repeat prescriptions link$")
    fun iClickOnGoToYourRepeatPrescriptionsLink() {
        nominatedPharmacyChangeSuccessPage.prescriptionsLink.click()
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

    @When("^I select high street pharmacy$")
    fun iSelectHighStreetPharmacy() {
        nominatedPharmacyChooseTypePage.highStreetPharmacyRadioButton.click()
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

    @Then("^I see the update nominated pharmacy interrupt page loaded$")
    fun iSeeUpdateNominatedPharmacyInterruptPageIsLoaded() {
        nominatedPharmacyInterruptPage.isLoaded(
                "Any outstanding prescriptions may still arrive at your current nominated pharmacy")
    }

    @Then("^I see the set nominated pharmacy interrupt page loaded$")
    fun iSeeSetNominatedPharmacyInterruptPageIsLoaded() {
        nominatedPharmacyInterruptPage.isLoaded("The pharmacy you choose is where your prescriptions will be sent")
    }

    @Then("^I click on the interrupt continue button$")
    fun iClickOnTheInterruptContinueButton() {
        nominatedPharmacyInterruptPage.continueButton.click()
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
                    dataItem.Address1, searchResults[index].address)

            val phoneNumberData = dataItem.primaryPhone()
            val phoneNumber = "Telephone: $phoneNumberData"
            if (phoneNumberData != null) {
                assertEquals(
                        "Phone number is not correct",
                        phoneNumber, searchResults[index].phoneNumber)
            }
        }
    }

    @Then("^I see list of online only pharmacies displayed on the result page$")
    fun iSeeOnlineOnlyPharmaciesOnResultsPage(){
        nominatedPharmacyResultsPage.isLoaded()
        val expectedData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        val searchResults = nominatedPharmacyResultsPage.getOnlinePharmacies()

        expectedData.forEachIndexed {
            index, dataItem ->
            assertEquals(
                    "Online Pharmacy name is not correct",
                    dataItem.OrganisationName, searchResults[index].pharmacyName)
            if(dataItem.URL != null){
                assertEquals(
                        "Online Pharmacy URL is not correct",
                        dataItem.URL, searchResults[index].website)
            }

            val phoneNumberData = dataItem.primaryPhone()
            val phoneNumber = "Telephone: $phoneNumberData"
            if (phoneNumberData != null) {
                assertEquals(
                        "Online Pharmacy Phone number is not correct",
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

    @Then("^I see your nominated pharmacy page with chosen pharmacy details$")
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
