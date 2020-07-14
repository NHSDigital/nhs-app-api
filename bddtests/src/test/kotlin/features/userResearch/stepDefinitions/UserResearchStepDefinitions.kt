package features.userResearch.stepDefinitions

import cucumber.api.java.en.Then
import pages.UserResearchPage

class UserResearchStepDefinitions {
    private lateinit var userResearchPage: UserResearchPage

    @Then("^the User Research page is displayed$")
    fun theUserResearchPageIsDisplayed() {
        userResearchPage.assertDisplayed()
    }
}
