package features.organDonation.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.data.organDonation.OrganDecisions
import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.getOrFail
import mocking.data.organDonation.set
import pages.organDonation.OrganDonationFindOutMoreAboutOrgansAndTissuePage
import pages.organDonation.OrganDonationSpecificOrganChoicePage

open class OrganDonationSpecificOrganChoiceStepDefinitions {

    lateinit var organDonationYourChoicePage: OrganDonationSpecificOrganChoicePage
    lateinit var organDonationFindOutMoreAboutOrgansAndTissuePage: OrganDonationFindOutMoreAboutOrgansAndTissuePage

    @When("^I choose which organs to donate")
    fun iChooseWhichOrgansToDonate() {
        val organsToDonate =
                OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED
                        .getOrFail<OrganDecisions>()
        organsToDonate.optIn.forEach { organ ->
            organDonationYourChoicePage.chooseOption(organ, true)
        }
        organsToDonate.optOut.forEach { organ ->
            organDonationYourChoicePage.chooseOption(organ, false)
        }
        OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING.set(organsToDonate)
    }

    @When("^I click the Find out more about organs and tissue link")
    fun thenIClickTheFindOutMoreAboutOrgansAndTissueLink(){
        organDonationYourChoicePage.moreAboutOrgansAndTissueLink.click()
    }

    @Then("^the Organ Donation Specific Organ Choice page is displayed")
    fun theOrganDonationYourChoicePageIsDisplayed() {
        organDonationYourChoicePage.assertDisplayed()
    }

    @Then("^the Find Out More About Organs And Tissue Page is displayed")
    fun thenTheFindOutMoreAboutOrgansAndTissuePageIsDisplayed() {
        organDonationFindOutMoreAboutOrgansAndTissuePage.assertDisplayed()
    }

    @Then("^a validation message is shown if a user attempts to continue without selecting a decision for all organs")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithoutSelectingADecisionForAllOrgans() {

        val errorMessage = arrayListOf("There's a problem",
                "Choose either ‘yes’ or ‘no’ for each organ.")

        organDonationYourChoicePage.assertAllOptionsUnselected()
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(errorMessage)

        val organToDonate =
                OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED
                        .getOrFail<OrganDecisions>().optIn.first()

        organDonationYourChoicePage.chooseOption(organToDonate, true)
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(errorMessage)

    }

    @Then("^a validation message is shown if a user attempts to continue with all specific organ options set to no")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithAllSpecificOrganOptionsSetToNo() {
        organDonationYourChoicePage.organOptions.forEach { option -> option.select(false) }
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertVisible(arrayListOf("There's a problem",
                "To continue, choose ‘yes’ for at least one organ."))
    }

    @Then("^my previous decisions are displayed on the Organ Donation Specific Organ Choice page")
    fun myPreviousDecisionsAreDisplayedOnTheOrganDonationSpecificOrganChoicePage() {
        val organsToDonate =
                OrganDonationSerenityHelpers.SOME_ORGANS_EXISTING
                        .getOrFail<OrganDecisions>()

        organsToDonate.optIn.forEach { organ ->
            organDonationYourChoicePage.assertOrganOption(organ, true)
        }
        organsToDonate.optOut.forEach { organ ->
            organDonationYourChoicePage.assertOrganOption(organ, false)
        }
        organsToDonate.notStated.forEach { organ ->
            organDonationYourChoicePage.assertOrganOption(organ, null)
        }
    }

    @Then("^no options on the Organ Donation Specific Organ Choice page are selected")
    fun noOptionsOnTheOrganDonationSpecificOrganChoicePageAreSelected() {
        organDonationYourChoicePage.assertAllOptionsUnselected()
    }
}
