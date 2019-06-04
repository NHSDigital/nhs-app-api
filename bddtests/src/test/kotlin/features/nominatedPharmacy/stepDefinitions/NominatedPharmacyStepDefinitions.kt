package features.nominatedPharmacy.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.nominatedPharmacy.steps.NominatedPharmacyDataSetupSteps
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.isVisible
import pages.nominatedPharmacy.CannotChangeNominatedPharmacyPage
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

    private lateinit var cantChangeNominatedPharmacyPage : CannotChangeNominatedPharmacyPage

    @Steps
    private lateinit var nominatedPharmacyDataSetupSteps: NominatedPharmacyDataSetupSteps

    @Given("^searching for pharmacies with (.*) has (\\d+) results")
    fun searchTextHasResults(searchText: String, numberOfResults: Int) {
        val data = NhsAzureSearchData.generatePharmacyData(numberOfResults)
        NominatedPharmacySerenityHelpers.SEARCH_RESULTS.set(data)

        nominatedPharmacyDataSetupSteps.setupWiremockForPharmacyTextSearch(searchText, data)
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

    @Given("^I don't have a nominated pharmacy of any type$")
    fun iDontHaveANominatedPharmacyOfAnyType() {
        nominatedPharmacyDataSetupSteps.setupNoNominatedPharmacy()
    }

    @When("^I click on change my nominated pharmacy link$")
    fun iClickOnChangeMyNominatedPharmacyLink() {
        nominatedPharmacyPage.changePharmacyLink.click()
    }

    @When("^I click on nominate a pharmacy link$")
    fun iClickOnNominateAPharmacyLink() {
        nominatedPharmacyPage.nominatedPharmacyLink.click()
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
                "Nominated pharmacy panel is not visible",
                prescriptionsPage.nominatedPharmacyPanel.isVisible)
    }

    @Then("^I see my nominated pharmacy on the prescriptions page$")
    fun iSeeMyNominatedPharmacyOnThePrescriptionsPage() {
        val updatedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        assertEquals(
                "Nominated pharmacy name is not correct",
                updatedPharmacy.OrganisationName, prescriptionsPage.nominatedPharmacyName.text)
    }

    @Then("^I see that I haven't nominated a pharmacy on the prescriptions page$")
    fun iSeeThatIHaventNominatedAPharmacyOnThePrescriptionsPage() {
        assertEquals("No nominated pharmacy", prescriptionsPage.nominatedPharmacyName.text)
    }

    @Then("^I see nominated pharmacy page loaded$")
    fun iAmRedirectedToNominatedPharmacyPage() {
        nominatedPharmacyPage.isLoadedWithPharmacy()
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

    @Then("^I see the nominate a pharmacy link$")
    fun iSeeNominatedAPharmacyLink() {
        assertTrue(
                "Nominate a pharmacy link is not visible",
                nominatedPharmacyPage.nominatedPharmacyLink.isVisible)
    }

    @Then("^I see search nominated pharmacy page loaded$")
    fun iSeeSearchNominatedPharmacyLoaded() {
        searchNominatedPharmacyPage.isLoaded()
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

    @Then("^I see cannot change pharmacy page$")
    fun iSeeCannotChangePharmacyPage() {
        cantChangeNominatedPharmacyPage.isLoaded()
    }

    @Then("^I see header with how to change pharmacy instruction$")
    fun iSeeHeaderWithHowToChangePharmacy() {
        assertTrue(
                "Instruction to change pharmacy is not visible",
                cantChangeNominatedPharmacyPage.changePharmacyInstruction.isVisible)
    }
}
