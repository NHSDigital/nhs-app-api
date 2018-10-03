package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.SerenityHelpers
import pages.MyAccountPage
import java.time.LocalDate
import java.time.format.TextStyle
import java.util.*


class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @When("^I click the Terms of use link$")
    fun iClickTheTermsAndConditionsLink() {
        myAccount.termsOfUseLink.element.click()
    }

    @When("^I click the Privacy policy link$")
    fun iClickThePrivacyPolicyLink() {
        myAccount.privacyPolicyLink.element.click()
    }

    @When("^I click the Cookies policy link$")
    fun iClickTheCookiesPolicyLink() {
        myAccount.cookiesPolicyLink.element.click()
    }

    @When("^I click the Open source licenses link$")
    fun iClickTheOpenSourceLicensesLink() {
        myAccount.openSourceLicensesLink.element.click()
    }

    @When("^I click the Help and support link$")
    fun iClickTheHelpAndSupportLink() {
        myAccount.helpAndSupportLink.element.click()
    }

    @Then("^I am on the My Account page$")
    fun iAmOnTheMyAccountPage() {
        myAccount.signOutButton.assertIsVisible()
        myAccount.isAboutUsHeaderVisible()
        myAccount.assertAllLinksVisible()
    }

    @Then("^I see my personal details")
    fun iSeeMyPersonalDetails() {

        val patient = SerenityHelpers.getPatient()
        myAccount.assertPersonalDetailsVisible(patient.formattedFullName(),
                patient.formattedDateOfBirthShort(),
                patient.formattedNHSNumber())
    }
}
