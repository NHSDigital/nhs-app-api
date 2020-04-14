package features.oneOneOneOnline.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.oneOneOneOnline.steps.CheckMySymptoms
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.NavBarNative

open class OneOneOneStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    lateinit var checkMySymptoms: CheckMySymptoms
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navBar: NavigationSteps

    @When("I press the A-Z symptoms header")
    fun iPressTheAtoZHelpHeader() {
        browser.storeCurrentTabCount()
        checkMySymptoms.clickConditionsHeader()
    }

    @When("I press the urgent help header")
    fun iPressUrgentHelpHeader() {
        browser.storeCurrentTabCount()
        checkMySymptoms.clickNHS111Header()
    }

    @When("I press the coronavirus header")
    fun iPressTheCoronaVirusHeader() {
        browser.storeCurrentTabCount()
        checkMySymptoms.clickCoronaVirusHeader()
    }

    @Then("^the Symptoms page is displayed")
    fun checkMySymptomsPageIsDisplayed() {
        checkMySymptoms.assertConditionsHeaderVisible()
        checkMySymptoms.assertNhs111HeaderVisible()
        checkMySymptoms.assertCoronaHeaderVisible()
    }

    @Then("^the Symptoms page header and navigation menu are correct$")
    fun checkMySymptomsPageHeaderAndNavigationMenuAreCorrect() {
        navBar.headerNative.assertIsVisible("Symptoms")
        navBar.assertSelectedTab(NavBarNative.NavBarType.SYMPTOMS)
    }
}
