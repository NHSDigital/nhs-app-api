package features.organDonation.stepDefinitions

import cucumber.api.java.en.When
import pages.assertIsVisible
import pages.organDonation.OrganDonationAdditionalDetailsPage
import pages.organDonation.OrganDonationCheckDetailsPage
import pages.organDonation.OrganDonationChoicePage
import pages.organDonation.OrganDonationFaithAndBeliefsPage
import pages.organDonation.OrganDonationViewRegistrationPage
import pages.organDonation.OrganDonationYourChoicePage

open class OrganDonationJourneyStepDefinitions {

    /**
     * The steps within this class are used to navigate through the Organ Donation Journey to a given page.
     * There are minimal assertions for each of the pages in the journey, so avoid using if a specific assertion is
     * needed on any of the interim pages.
     */

    lateinit var organDonationYourChoicePage: OrganDonationYourChoicePage
    lateinit var organDonationChoicePage: OrganDonationChoicePage
    lateinit var organDonationFaithAndBeliefsPage: OrganDonationFaithAndBeliefsPage
    lateinit var organDonationAdditionalDetailsPage: OrganDonationAdditionalDetailsPage
    lateinit var organDonationCheckDetailsPage: OrganDonationCheckDetailsPage
    lateinit var organDonationViewRegistrationPage: OrganDonationViewRegistrationPage

    @When("I follow the opt-in journey to the '(.*)' page")
    fun iFollowTheOrganDonationOptInJourney(pageTitle: String) {
        organDonationChoicePage.yesButton.assertIsVisible().click()
        organDonationYourChoicePage.assertDisplayed()
        if(pageTitle == "Your Choice"){
            return
        }
        navigateToFaithAndBeliefsPage(pageTitle)
    }

    @When("I follow the opt-out journey to the '(.*)' page")
    fun iFollowTheOrganDonationOptOutJourney(pageTitle: String) {
        organDonationChoicePage.noButton.assertIsVisible().click()
        navigateToAdditionalDetailsPageForOptOutJourney(pageTitle)
    }

    private fun navigateToFaithAndBeliefsPage(pageTitle: String){
        organDonationYourChoicePage.radioButtons.button(organDonationYourChoicePage.allOfMyOrgans).select()
        organDonationYourChoicePage.clickContinue()
        organDonationFaithAndBeliefsPage.assertDisplayed()
        if(pageTitle == "Faith And Beliefs"){
            return
        }
        navigateToAdditionalDetailsPageForOptInJourney(pageTitle)
    }

    private fun navigateToAdditionalDetailsPageForOptInJourney(pageTitle: String) {
        organDonationFaithAndBeliefsPage.radioButtons.button("Prefer not to say").select()
        organDonationFaithAndBeliefsPage.clickContinue()
        organDonationAdditionalDetailsPage.assertDisplayed()
        if(pageTitle == "Additional Details"){
            return
        }
        navigateToCheckDetailsPage(pageTitle)
    }

    private fun navigateToAdditionalDetailsPageForOptOutJourney(pageTitle: String){
        organDonationAdditionalDetailsPage.assertDisplayed()
        if(pageTitle == "Additional Details"){
            return
        }
        navigateToCheckDetailsPage(pageTitle)
    }

    private fun navigateToCheckDetailsPage(pageTitle: String){
        organDonationAdditionalDetailsPage.clickContinue()
        organDonationCheckDetailsPage.assertDisplayed()
        if(pageTitle == "Check Details"){
            return
        }
        navigateToConfirmationPage()
    }

    private fun navigateToConfirmationPage(){
        organDonationCheckDetailsPage.accuracyCheckBox.click()
        organDonationCheckDetailsPage.privacyStatementCheckBox.click()
        organDonationCheckDetailsPage.clickSubmit()
        organDonationViewRegistrationPage.assertDisplayed()
    }
}

