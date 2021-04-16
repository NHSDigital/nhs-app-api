package features.gpSessionOnDemand.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import utils.SerenityHelpers
import pages.loggedOut.LoginStubPage

class GpSessionOnDemandStepDefinitions {
    private lateinit var loginStubPage: LoginStubPage

    @Given("^I have a Decoupled GP User Session$")
    fun iHaveADecoupledGpUserSession() {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet("DECOUPLED", true)
    }

    @Then("^I SSO to NhsLogin$")
    fun iSsoToNhsLogin() {
        loginStubPage.signIn(SerenityHelpers.getPatient())
    }
}
