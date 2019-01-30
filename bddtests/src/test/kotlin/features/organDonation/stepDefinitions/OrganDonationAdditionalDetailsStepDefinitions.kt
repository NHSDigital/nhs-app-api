package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import net.serenitybdd.core.Serenity
import pages.navigation.HeaderNative
import pages.organDonation.OrganDonationAdditionalDetailsPage
import utils.SerenityHelpers

open class OrganDonationAdditionalDetailsStepDefinitions {

    lateinit var header: HeaderNative
    lateinit var organDonationAdditionalDetailsPage: OrganDonationAdditionalDetailsPage

    @When("I select an ethnicity to record for organ donation")
    fun iSelectAnEthnicityToRecordForOrganDonation() {
        val patient = SerenityHelpers.getPatient()
        organDonationAdditionalDetailsPage.ethnicitySelector.selectByText(
                patient.organDonationDemographics.ethnicity.value)
    }

    @When("I select a religion to record for organ donation")
    fun iSelectAReligionToRecordForOrganDonation() {
        val patient = SerenityHelpers.getPatient()
        organDonationAdditionalDetailsPage.religionSelector.selectByText(
                patient.organDonationDemographics.religion.value)
    }

    @Then("^the Organ Donation Decision Additional Details page is displayed")
    fun theOrganDonationDecisionAdditionalDetailsPageIsDisplayed() {
        organDonationAdditionalDetailsPage.assertDisplayed()

        val expectedReligions = Serenity.sessionVariableCalled<ArrayList<String>>("ReferenceReligions")

        if (!expectedReligions.contains("Please select")) {
            expectedReligions.add("Please select")
        }
        val expectedEthnicities = Serenity.sessionVariableCalled<ArrayList<String>>("ReferenceEthnicities")
        if (!expectedEthnicities.contains("Please select")) {
            expectedEthnicities.add("Please select")
        }

        organDonationAdditionalDetailsPage.ethnicitySelector.assertContents(expectedEthnicities)
        organDonationAdditionalDetailsPage.religionSelector.assertContents(expectedReligions)
    }
}
