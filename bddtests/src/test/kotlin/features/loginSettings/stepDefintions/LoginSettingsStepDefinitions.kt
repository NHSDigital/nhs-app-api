package features.loginSettings.stepDefintions

import cucumber.api.java.en.Then
import pages.account.LoginSettingsPage

class LoginSettingsStepDefinitions {

    lateinit var loginSettings: LoginSettingsPage

    @Then("I see the login options page")
    fun iSeeTheLoginAndPasswordOptionsPage() {
        loginSettings.assertDisplayed()
    }
}
