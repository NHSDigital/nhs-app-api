package features.nominatedPharmacy.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.nominatedPharmacy.steps.NominatedPharmacyDataSetupSteps
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import models.nominatedPharmacy.Postcode
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.PrescriptionsHubPage
import pages.assertIsVisible
import pages.nominatedPharmacy.ConfirmNominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyChangeSuccessPage
import pages.nominatedPharmacy.NominatedPharmacyChooseTypePage
import pages.nominatedPharmacy.NominatedPharmacyPage
import pages.nominatedPharmacy.NominatedPharmacyResultsPage
import pages.prescription.ViewOrdersPrescriptionsPage
import pages.text
import utils.SerenityHelpers
import utils.getOrFail
import utils.set

class NominatedPharmacyStepDefinitions {

    private lateinit var nominatedPharmacyPage: NominatedPharmacyPage

    private lateinit var viewOrdersPrescriptionsPage: ViewOrdersPrescriptionsPage

    private lateinit var confirmNominatedPharmacyPage: ConfirmNominatedPharmacyPage

    private lateinit var nominatedPharmacyResultsPage: NominatedPharmacyResultsPage

    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    private lateinit var nominatedPharmacyChangeSuccessPage: NominatedPharmacyChangeSuccessPage

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
    fun iHaveANominatedPharmacy(pharmacyType: String, odsCode: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacy(pharmacyType, odsCode)
    }

    @Given("^I have a (.*) typed nominated pharmacy$")
    fun iHaveANominatedPharmacy(pharmacyType: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacy(pharmacyType, SerenityHelpers.getPatient().odsCode)
    }

    @Given("^I have a P1 typed Internet pharmacy with (.*) OdsCode$")
    fun iHaveAnInternetPharmacy(odsCode: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacyWithInternetPharmacy(odsCode)
    }

    @Given("^I have a (.*) typed nominated pharmacy with (.*) OdsCode and nhsNumber (.*) is returned$")
    fun iHaveANomPharmButDifferentNhsNumberIsReturned(pharmacyType: String, odsCode: String, nhsNumber: String) {
        nominatedPharmacyDataSetupSteps.setupNominatedPharmacyWithDifferentNhsNumber(pharmacyType, odsCode, nhsNumber)
    }

    @Given("^I have a (.*) typed nominated pharmacy with (.*) OdsCode and (.*) ConfidentialityCode$")
    fun iHaveANomPharmAndConfidentialityCode(pharmacyType: String, odsCode: String, confidentialityCode: String) {
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

    @When("^I click on the nominated pharmacy panel when pharmacy is set$")
    fun iClickTheNominatedPharmacyPanelWhenPharmacyIsSet() {
        prescriptionsHubPage.nominatedPharmacyLink.click()
    }

    @When("^I click on the nominated pharmacy panel when pharmacy is not set$")
    fun iClickTheNominatedPharmacyPanelWhenPharmacyIsNotSet() {
        prescriptionsHubPage.nominatedPharmacyLink.click()
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

        Assert.assertEquals(
                "Organisation name is not correct",
                myNominatedPharmacy.OrganisationName, nominatedPharmacyChangeSuccessPage.pharmacyName.text)

        Assert.assertEquals(
                "Address Line 1 is not correct",
                myNominatedPharmacy.Address1, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine1.text)


        Assert.assertEquals(
                "Address Line 2 is not correct",
                myNominatedPharmacy.Address2, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine2.text)

        Assert.assertEquals(
                "Address Line 3 is not correct",
                myNominatedPharmacy.Address3, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine3.text)

        Assert.assertEquals(
                "City is not correct",
                myNominatedPharmacy.City, nominatedPharmacyChangeSuccessPage.pharmacyCity.text)

        Assert.assertEquals(
                "County is not correct",
                myNominatedPharmacy.County, nominatedPharmacyChangeSuccessPage.pharmacyCounty.text)

        Assert.assertEquals(
                "Postcode is not correct",
                myNominatedPharmacy.Postcode, nominatedPharmacyChangeSuccessPage.pharmacyPostcode.text)

        val phoneNumber = myNominatedPharmacy.primaryPhone()

        if (phoneNumber != null) {
            Assert.assertEquals(
                    "Phone number is not correct",
                    "Telephone: " + phoneNumber, nominatedPharmacyChangeSuccessPage.pharmacyPhoneNumber.text)
        }
    }

    @When("^I click on the go to your prescriptions orders link$")
    fun iClickOnGoToYourRepeatPrescriptionsLink() {
        nominatedPharmacyChangeSuccessPage.prescriptionsLink.click()
    }

    @Then("^I see the nominated pharmacy panel on the prescriptions hub page$")
    fun iSeeTheNominatedPharmacyBanner() {
        prescriptionsHubPage.nominatedPharmacyLink.assertIsVisible("Nominated pharmacy panel is visible")
    }

    @Then("^I see my nominated pharmacy on the prescriptions hub page$")
    fun iSeeMyNominatedPharmacyOnThePrescriptionsPage() {
        val updatedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        Assert.assertEquals(
                "Nominated pharmacy name is not correct",
                updatedPharmacy.OrganisationName, prescriptionsHubPage.pharmacyName.text)
    }

    @Then("^I see the help text for no set nominated pharmacy$")
    fun iSeeTheHelpTextForNoSetNominatedPharmacy() {
        Assert.assertEquals(
                "Help text for no set pharmacy is missing",
                "You do not have a nominated pharmacy.",
                viewOrdersPrescriptionsPage.noSetNominatedPharmacyHelpText.text)
    }

    @Then("^I see my nominated pharmacy on the view orders page$")
    fun iSeeMyNominatedPharmacyOnTheViewOrdersPage() {
        val updatedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        Assert.assertEquals(
                "Nominated pharmacy name is not correct",
                updatedPharmacy.OrganisationName, viewOrdersPrescriptionsPage.getNominatedPharmacyName())
    }

    @Then("^I see that I haven't nominated a pharmacy on the prescriptions page$")
    fun iSeeThatIHaventNominatedAPharmacyOnThePrescriptionsPage() {
        Assert.assertEquals("Choose a pharmacy for your prescriptions to be sent to",
                prescriptionsHubPage.pharmacyName.text)
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
        nominatedPharmacyPage.changePharmacyLink
                .assertIsVisible("Change my nominated pharmacy link is not visible")
    }

    @Then("^I see the no results found page$")
    fun iSeeNoResultsFound() {
        nominatedPharmacyResultsPage.showsNoResultsFoundHeader()
    }

    @Then("^I see list of pharmacies displayed on the result page$")
    fun iSeePharmaciesOnResultsPage() {
        nominatedPharmacyResultsPage.assertIsLoaded()
        val expectedData = NominatedPharmacySerenityHelpers
                .SEARCH_RESULTS
                .getOrFail<NhsAzureSearchOrganisationReply>().value

        val searchResults = nominatedPharmacyResultsPage.getPharmacies()

        expectedData.forEachIndexed { index, dataItem ->
            Assert.assertEquals(
                    "Pharmacy name is not correct",
                    dataItem.OrganisationName, searchResults[index].pharmacyName)
            Assert.assertEquals(
                    "Pharmacy address is not correct",
                    dataItem.Address1, searchResults[index].address)

            val phoneNumberData = dataItem.primaryPhone()
            val phoneNumber = "Telephone: $phoneNumberData"
            if (phoneNumberData != null) {
                Assert.assertEquals(
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
        Assert.assertEquals(
                "Organisation name is not correct",
                selectedPharmacy.OrganisationName, confirmNominatedPharmacyPage.pharmacyName.text)

        Assert.assertEquals(
                "Address Line 1 is not correct",
                selectedPharmacy.Address1, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine1.text)


        Assert.assertEquals(
                "Address Line 2 is not correct",
                selectedPharmacy.Address2, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine2.text)

        Assert.assertEquals(
                "Address Line 3 is not correct",
                selectedPharmacy.Address3, nominatedPharmacyChangeSuccessPage.pharmacyAddressLine3.text)

        Assert.assertEquals(
                "City is not correct",
                selectedPharmacy.City, nominatedPharmacyChangeSuccessPage.pharmacyCity.text)

        Assert.assertEquals(
                "County is not correct",
                selectedPharmacy.County, nominatedPharmacyChangeSuccessPage.pharmacyCounty.text)

        Assert.assertEquals(
                "Postcode is not correct",
                selectedPharmacy.Postcode, nominatedPharmacyChangeSuccessPage.pharmacyPostcode.text)

        val phoneNumber = selectedPharmacy.primaryPhone()

        if (phoneNumber != null) {
            Assert.assertEquals(
                    "Phone number is not correct",
                    "Telephone: " + phoneNumber, confirmNominatedPharmacyPage.pharmacyPhoneNumber.text)
        }
    }

    fun checkPharmacyDetailsAreCorrect() {
        val selectedPharmacy = NominatedPharmacySerenityHelpers
                .MY_NOMINATED_PHARMACY
                .getOrFail<NhsAzureSearchOrganisationItem>()

        Assert.assertEquals(
                "Pharmacy name is not correct",
                selectedPharmacy.OrganisationName, nominatedPharmacyPage.pharmacyName.text)

        Assert.assertEquals(
                "Address Line 1 is not correct",
                selectedPharmacy.Address1, nominatedPharmacyPage.pharmacyAddressLine1.text)


        Assert.assertEquals(
                "Address Line 2 is not correct",
                selectedPharmacy.Address2, nominatedPharmacyPage.pharmacyAddressLine2.text)

        Assert.assertEquals(
                "Address Line 3 is not correct",
                selectedPharmacy.Address3, nominatedPharmacyPage.pharmacyAddressLine3.text)

        Assert.assertEquals(
                "City is not correct",
                selectedPharmacy.City, nominatedPharmacyPage.pharmacyCity.text)

        Assert.assertEquals(
                "County is not correct",
                selectedPharmacy.County, nominatedPharmacyPage.pharmacyCounty.text)

        Assert.assertEquals(
                "Postcode is not correct",
                selectedPharmacy.Postcode, nominatedPharmacyPage.pharmacyPostcode.text)

        val phoneNumber = selectedPharmacy.primaryPhone()

        if (phoneNumber != null) {
            Assert.assertEquals(
                    "Phone number is not correct",
                    "Telephone: " + phoneNumber, nominatedPharmacyPage.pharmacyPhoneNumber.text)
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
        nominatedPharmacyPage.cannotChangeDispensingPractiseInformationLine1
                .assertIsVisible("Instruction 1 to change pharmacy is not visible")
        nominatedPharmacyPage.cannotChangeDispensingPractiseInformationLine2
                .assertIsVisible("Instruction 2 to change pharmacy is not visible")
    }
}
