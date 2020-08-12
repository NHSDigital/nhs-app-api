package features.userResearch.stepDefinitions

import io.cucumber.java.en.Then
import pages.UserResearchPage

class UserResearchStepDefinitions {
    private lateinit var userResearchPage: UserResearchPage

    @Then("^the User Research page is displayed$")
    fun theUserResearchPageIsDisplayed() {
        userResearchPage.assertDisplayed()
    }
}
