package features.gpSessionOnDemand.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import pages.ErrorDialogPage
import utils.SerenityHelpers
import pages.loggedOut.LoginStubPage

class GpSessionOnDemandStepDefinitions {
    private lateinit var loginStubPage: LoginStubPage
    private lateinit var errorDialogPage: ErrorDialogPage

    @Given("^I have a Decoupled GP User Session$")
    fun iHaveADecoupledGpUserSession() {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet("DECOUPLED", true)
    }

    @Then("^I SSO to NhsLogin$")
    fun iSsoToNhsLogin() {
        loginStubPage.signIn(SerenityHelpers.getPatient())
    }

    @Then("^I see the generic try again error message$")
    fun iSeeTheGenericTryAgainErrorMessage() {
        errorDialogPage
            .assertParagraphText("This service is unavailable, it may be a temporary problem.")
            .assertPageHeader("Sorry, there is a problem")
            .assertPageTitle("Sorry, there is a problem")
    }
}
