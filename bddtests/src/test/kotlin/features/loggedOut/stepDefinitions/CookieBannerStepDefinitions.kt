package features.loggedOut.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.loggedOut.steps.CookieBannerSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.CookieSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.CheckMySymptomsPage
import pages.loggedOut.CookieBanner
import pages.loggedOut.LoginPage

private const val DO_SEE = "see"
private const val DO_NOT_SEE = "do not see"
private const val WILL = "will"
private const val WILL_NOT = "will not"

class CookieBannerStepDefinitions {

    @Steps
    private lateinit var browserSteps: BrowserSteps
    @Steps
    private lateinit var cookieSteps : CookieSteps
    @Steps
    lateinit var cookieBannerSteps: CookieBannerSteps

    private lateinit var loginPage: LoginPage
    private lateinit var checkMySymptomsPage: CheckMySymptomsPage
    private lateinit var cookieBanner: CookieBanner

    @Given("^session storage is cleared$")
    fun sessionStorageIsCleared() {
        cookieSteps.clearSessionStorage("hasClosedCookies")
        browserSteps.refreshPage()
    }

    @When("^I am on the login logged-out page$")
    fun iAmOnTheLoginLoggedOutPage() {
        browserSteps.goToApp()
    }

    @When("^I reopen the app$")
    fun iReopenTheApp() {
        browserSteps.closeApp()
        browserSteps.goToApp()
    }

    @When("^I close the app$")
    fun iCloseTheApp() {
        browserSteps.closeApp()
    }

    @When("^I refresh the page$")
    fun iRefreshThePage() {
        browserSteps.refreshPage()
    }

    @When("^I am on the check your symptoms logged-out page$")
    fun iAmOnTheCheckYourSymptomsLoggedOutPage(){
        browserSteps.goToApp()
        loginPage.symptomsButton.click()
        checkMySymptomsPage.isConditionsHeaderVisible()
        checkMySymptomsPage.isNhs111HeaderVisible()
    }

    @When("^I close the cookie banner$")
    fun iCloseTheCookieBanner() {
        cookieBanner.cookieBannerClose.click()
    }

    @Then("^I ($DO_NOT_SEE|$DO_SEE) the cookie banner$")
    fun iSeeTheCookieBanner(visibility: String) {
        when (visibility) {
            DO_NOT_SEE -> cookieBannerSteps.iSeeCookieBanner(false)
            DO_SEE -> cookieBannerSteps.iSeeCookieBanner(true)
        }
    }

    @Then("^I do not see the cookie banner with javascript disabled$")
    fun iDoNotSeeTheCookieBannerWithJsDisabled() {
        cookieBannerSteps.iDoNotSeeCookieBannerNoJs()
    }

    @Then("^session storage is not present$")
    fun sessionStorageIsNotPresent() {
        Assert.assertNull("Cookie exists. ",  cookieSteps.hasClosedCookies("hasClosedCookies"))
    }

    @Then("^session storage is created with the value of true$")
    fun sessionStorageIsCreatedWithTheValueOfTrue() {
        Assert.assertEquals("Session storage exists. ","true", cookieSteps.hasClosedCookies("hasClosedCookies"))
    }

    @Then("^pages ($WILL|$WILL_NOT) display the cookie banner$")
    fun pagesWillNotDisplayTheCookieBanner(visibility: String, urls: List<String>) {
        for (url in urls) {
            val fullUrl = Config.instance.url + url
            browserSteps.browseTo(fullUrl)
            when (visibility) {
                WILL -> cookieBannerSteps.iSeeCookieBanner(true)
                WILL_NOT -> cookieBannerSteps.iSeeCookieBanner(false)
            }
        }
    }
}
