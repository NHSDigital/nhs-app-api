package features.linkedProfiles.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.switchProfiles.SwitchProfileData
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HomePage
import pages.switchProfiles.SwitchProfilesPage
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail

class SwitchProfileStepDefinitions {
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps

    lateinit var home: HomePage

    private lateinit var switchProfilePage: SwitchProfilesPage

    val mockingClient = MockingClient.instance

    @Then("^the switch profiles page is displayed$")
    fun theSwitchProfilesPageIsDisplayed() {
        val selectedPatient = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        switchProfilePage.isLoaded(selectedPatient.profile.formattedFullName())
    }

    @And("^the correct proxy user details are displayed$")
    fun correctProxyUserDetailsAreDisplayed() {
        val displayedProxyDetails = switchProfilePage.getDisplayedProxyDetails()
        checkDisplayedValuesAreCorrect(displayedProxyDetails)
    }

    private fun checkDisplayedValuesAreCorrect(displayedProxyDetails: SwitchProfileData)
    {
        val currentProxyPatient = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        // proxy patient was selected in previous step
        Assert.assertEquals(
                "Proxy patient age did not match",
                currentProxyPatient.profile.formattedAge(), displayedProxyDetails.age)

        Assert.assertEquals(
                "Proxy patient gp practice did not match",
                currentProxyPatient.gpPracticeName, displayedProxyDetails.gpPracticeName)
    }

    @When("I click the Switch to my profile button for the main user")
    fun iClickTheSwitchToThisProfileButtonForTheProxyUser() {
        switchProfilePage.switchToMyProfileButton.click()
    }
}
