package features.loggedOut.stepDefinitions

import io.cucumber.java.en.Then
import pages.assertElementNotPresent
import pages.isDisplayed
import pages.loggedOut.LoginPage

class LoggedOutHomePageStepDefinitions {

    private lateinit var loginPage: LoginPage

    @Then("^I see a list of other services I can use without logging in$")
    fun iSeeAListOfServicesICanUseWithoutLoggingIn() {
        loginPage.otherServicesDiv.isDisplayed
    }

    @Then("^I see Before you start information$")
    fun iSeeBeforeYouStartInformation() {
        loginPage.beforeYouStartDiv.isDisplayed
    }

    @Then("^I see the Download app panel$")
    fun iSeeTheDownloadAppPanel() {
        loginPage.downloadAppPanel.isDisplayed
    }


    @Then("^I see the desktop specific information displayed$")
    fun iSeeDesktopSpecificInformationDisplayed() {
        loginPage.desktopSpecificInformation.isDisplayed
    }

    @Then("^I do not see desktop specific information displayed$")
    fun iDoNotSeeDesktopSpecificInformationDisplayed() {
        loginPage.desktopSpecificInformation.assertElementNotPresent()
    }

}
