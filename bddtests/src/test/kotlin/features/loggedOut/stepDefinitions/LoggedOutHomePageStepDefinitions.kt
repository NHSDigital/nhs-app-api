package features.loggedOut.stepDefinitions

import io.cucumber.java.en.Then
import pages.assertElementNotPresent
import pages.assertIsDisplayed
import pages.loggedOut.LoginPage

class LoggedOutHomePageStepDefinitions {

    private lateinit var loginPage: LoginPage

    @Then("^I see a list of other services I can use without logging in$")
    fun iSeeAListOfServicesICanUseWithoutLoggingIn() {
        loginPage.otherServicesDiv.assertIsDisplayed("Expected Other Services")
    }

    @Then("^I see Before you start information$")
    fun iSeeBeforeYouStartInformation() {
        loginPage.beforeYouStartDiv.assertIsDisplayed("Expected Before You Start Information")
    }

    @Then("^I see the Download app panel$")
    fun iSeeTheDownloadAppPanel() {
        loginPage.downloadAppPanel.assertIsDisplayed("Expected Download App Panel")
    }


    @Then("^I see the desktop specific information displayed$")
    fun iSeeDesktopSpecificInformationDisplayed() {
        loginPage.desktopSpecificInformation.assertIsDisplayed("Expected Desktop Specific Info")
    }

    @Then("^I do not see desktop specific information displayed$")
    fun iDoNotSeeDesktopSpecificInformationDisplayed() {
        loginPage.desktopSpecificInformation.assertElementNotPresent()
    }

}
