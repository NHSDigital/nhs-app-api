package features.organDonation.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.data.organDonation.OrganDecisions
import mocking.data.organDonation.OrganDonationSerenityHelpers
import pages.organDonation.OrganDonationFindOutMoreAboutOrgansAndTissuePage
import pages.organDonation.OrganDonationSpecificOrganChoicePage
import utils.getOrFail
import utils.set

open class OrganDonationSpecificOrganChoiceStepDefinitions {

    lateinit var organDonationYourChoicePage: OrganDonationSpecificOrganChoicePage
    lateinit var organDonationFindOutMoreAboutOrgansAndTissuePage: OrganDonationFindOutMoreAboutOrgansAndTissuePage
    private var organDonationAllMissingChoices: String = """
        |Select either 'yes' or 'no' for heart
        |Select either 'yes' or 'no' for lungs
        |Select either 'yes' or 'no' for kidney
        |Select either 'yes' or 'no' for liver
        |Select either 'yes' or 'no' for corneas
        |Select either 'yes' or 'no' for pancreas
        |Select either 'yes' or 'no' for tissue
        |Select either 'yes' or 'no' for small bowel
        """.trimMargin()
    private var organDonationAllMissingChoicesButHeart: String = """
        |Select either 'yes' or 'no' for lungs
        |Select either 'yes' or 'no' for kidney
        |Select either 'yes' or 'no' for liver
        |Select either 'yes' or 'no' for corneas
        |Select either 'yes' or 'no' for pancreas
        |Select either 'yes' or 'no' for tissue
        |Select either 'yes' or 'no' for small bowel
        """.trimMargin()

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
        val errorMessage = arrayListOf("There is a problem", organDonationAllMissingChoices)

        organDonationYourChoicePage.assertAllOptionsUnselected()
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertFormErrorSummaryVisible(errorMessage)

        val organToDonate = OrganDonationSerenityHelpers.SOME_ORGANS_UPDATED.getOrFail<OrganDecisions>().optIn.first()
        val errorMessageWithoutHeart = arrayListOf("There is a problem", organDonationAllMissingChoicesButHeart)
        
        organDonationYourChoicePage.chooseOption(organToDonate, true)
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertFormErrorSummaryVisible(errorMessageWithoutHeart)
    }

    @Then("^a validation message is shown if a user attempts to continue with all specific organ options set to no")
    fun aValidationMessageIsShownIfAUserAttemptsToContinueWithAllSpecificOrganOptionsSetToNo() {
        organDonationYourChoicePage.organOptions.forEach { option -> option.select(false) }
        organDonationYourChoicePage.clickContinue()
        organDonationYourChoicePage.validationBanner.assertFormErrorSummaryVisible(arrayListOf("There is a problem",
                "Select 'yes' for at least one organ"))
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
