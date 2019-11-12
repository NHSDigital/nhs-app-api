package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.account.MyAccountPage
import utils.SerenityHelpers

class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @When("I click the Notifications link on the Account page")
    fun iClickTheNotificationsLinkOnTheAccountPage(){
        myAccount.settings.notifications.click()
    }

    @Then("^the Account page for mobile devices is displayed$")
    fun theAccountPageForMobileDevicesIsDisplayed() {
        myAccount.assertDisplayedForMobile()
    }

    @Then("^the Account page is displayed$")
    fun theAccountPageIsDisplayed() {
        myAccount.assertDisplayed()
    }

    @Then("^I see my personal details")
    fun iSeeMyPersonalDetails() {
        val patient = SerenityHelpers.getPatient()
        myAccount.personalDetails.assertVisible(patient.formattedFullName(),
                patient.formattedDateOfBirth(),
                patient.formattedNHSNumber())
    }

    @Then("the Account Settings are available")
    fun theAccountSettingsAreAvailable(){
        myAccount.settings.assertLinksPresent()
    }

    @Then("there are no Account Settings available")
    fun thereAreNoAccountSettingsAvailable(){
        myAccount.settings.assertNotDisplayed()
    }

    @Then("the link to Notifications is not available on the Account page")
    fun theLinkToNotificationsIsNotAvailableOnTheAccountPage() {
        myAccount.settings.assertNotDisplayed()
    }
}
