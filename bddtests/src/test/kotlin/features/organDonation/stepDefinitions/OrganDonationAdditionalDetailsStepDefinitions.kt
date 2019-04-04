package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.organDonation.models.OrganDonationDemographics
import pages.navigation.HeaderNative
import pages.organDonation.OrganDonationAdditionalDetailsPage
import utils.getOrFail

open class OrganDonationAdditionalDetailsStepDefinitions {

    lateinit var header: HeaderNative
    lateinit var organDonationAdditionalDetailsPage: OrganDonationAdditionalDetailsPage

    @When("I select an ethnicity to record for organ donation")
    fun iSelectAnEthnicityToRecordForOrganDonation() {
        organDonationAdditionalDetailsPage.ethnicitySelector.selectByText(
                OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED
                        .getOrFail<OrganDonationDemographics>().ethnicity.value)
    }

    @When("I select a religion to record for organ donation")
    fun iSelectAReligionToRecordForOrganDonation() {
        organDonationAdditionalDetailsPage.religionSelector.selectByText(
                OrganDonationSerenityHelpers.DEMOGRAPHICS_UPDATED
                        .getOrFail<OrganDonationDemographics>().religion.value)
    }

    @Then("^the Organ Donation Decision Additional Details page is displayed")
    fun theOrganDonationDecisionAdditionalDetailsPageIsDisplayed() {
        organDonationAdditionalDetailsPage.assertDisplayed()
        val expectedReligions = arrayListOf("Please select").plus(
                OrganDonationSerenityHelpers.REFERENCE_RELIGIONS.getOrFail<ArrayList<String>>())
        val expectedEthnicities = arrayListOf("Please select").plus(
                OrganDonationSerenityHelpers.REFERENCE_ETHNICITIES.getOrFail<ArrayList<String>>())

        organDonationAdditionalDetailsPage.ethnicitySelector.assertContents(expectedEthnicities)
        organDonationAdditionalDetailsPage.religionSelector.assertContents(expectedReligions)
    }
}
