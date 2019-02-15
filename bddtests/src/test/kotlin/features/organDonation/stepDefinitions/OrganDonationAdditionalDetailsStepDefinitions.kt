package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.OrganDonationSerenityHelpers.Companion.ORGAN_DONATION_REFERENCE_ETHNICITIES
import mocking.data.organDonation.OrganDonationSerenityHelpers.Companion.ORGAN_DONATION_REFERENCE_RELIGIONS
import net.serenitybdd.core.Serenity
import pages.navigation.HeaderNative
import pages.organDonation.OrganDonationAdditionalDetailsPage

open class OrganDonationAdditionalDetailsStepDefinitions {

    lateinit var header: HeaderNative
    lateinit var organDonationAdditionalDetailsPage: OrganDonationAdditionalDetailsPage

    @When("I select an ethnicity to record for organ donation")
    fun iSelectAnEthnicityToRecordForOrganDonation() {
        organDonationAdditionalDetailsPage.ethnicitySelector.selectByText(
                OrganDonationSerenityHelpers.getOrganDonationDemographics().ethnicity.value)
    }

    @When("I select a religion to record for organ donation")
    fun iSelectAReligionToRecordForOrganDonation() {
        organDonationAdditionalDetailsPage.religionSelector.selectByText(
                OrganDonationSerenityHelpers.getOrganDonationDemographics().religion.value)
    }

    @Then("^the Organ Donation Decision Additional Details page is displayed")
    fun theOrganDonationDecisionAdditionalDetailsPageIsDisplayed() {
        organDonationAdditionalDetailsPage.assertDisplayed()

        val expectedReligions = Serenity.sessionVariableCalled<ArrayList<String>>(
                ORGAN_DONATION_REFERENCE_RELIGIONS)

        if (!expectedReligions.contains("Please select")) {
            expectedReligions.add("Please select")
        }
        val expectedEthnicities = Serenity.sessionVariableCalled<ArrayList<String>>(
                ORGAN_DONATION_REFERENCE_ETHNICITIES)
        if (!expectedEthnicities.contains("Please select")) {
            expectedEthnicities.add("Please select")
        }

        organDonationAdditionalDetailsPage.ethnicitySelector.assertContents(expectedEthnicities)
        organDonationAdditionalDetailsPage.religionSelector.assertContents(expectedReligions)
    }
}
