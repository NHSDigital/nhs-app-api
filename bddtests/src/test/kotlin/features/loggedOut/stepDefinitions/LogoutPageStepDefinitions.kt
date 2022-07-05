package features.loggedOut.stepDefinitions;

import io.cucumber.java.en.Then;
import pages.loggedOut.LogoutPage;
import pages.assertIsDisplayed

class LogoutPageStepDefinitions {
    lateinit var logoutPage: LogoutPage

    @Then("^I see the Logout page displayed$")
    fun iSeeTheLogoutPageDisplayed() {
        logoutPage.logoutPageText.assertIsDisplayed("Expected to see the Logout Page")
    }

}
