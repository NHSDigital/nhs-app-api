package features.loggedOut.stepDefinitions

import config.Config
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.loggedOut.steps.CookieBannerSteps
import features.sharedSteps.BrowserSteps
import features.throttling.stepDefinitions.GpFinderPageStepDefinitions
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.CheckMySymptomsPage
import pages.loggedOut.CookieBanner
import pages.loggedOut.LoginPage

private const val COOKIE_OPTIONS_COOKIE_NAME = "nhso.cookie_options"
private const val DO_SEE = "see"
private const val DO_NOT_SEE = "do not see"
private const val WILL = "will"
private const val WILL_NOT = "will not"

class CookieBannerStepDefinitions {

    @Steps
    private lateinit var browserSteps: BrowserSteps
    @Steps
    lateinit var cookieBannerSteps: CookieBannerSteps
    @Steps
    lateinit var gpFinderSteps: GpFinderPageStepDefinitions

    private lateinit var loginPage: LoginPage
    private lateinit var checkMySymptomsPage: CheckMySymptomsPage
    private lateinit var cookieBanner: CookieBanner

    @When("^I am on the login logged-out page$")
    fun iAmOnTheLoginLoggedOutPage() {
        browserSteps.goToApp()
    }

    @When("^I am on the check your symptoms logged-out page$")
    fun iAmOnTheCheckYourSymptomsLoggedOutPage(){
        browserSteps.goToApp()
        loginPage.symptomsButton.click()
        checkMySymptomsPage.isConditionsHeaderVisible()
        checkMySymptomsPage.isNhs111HeaderVisible()
    }

    @When("^I am on the gp finder logged-out page$")
    fun iAmOnTheGpFinderLoggedOutPage(){
        gpFinderSteps.iHaveNotLoggedInAndIHaveNotPreviouslySelectedMyGPPractice()
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

    @Then("^no cookie is created that would hide this banner$")
    fun noCookieIsCreatedThatWouldHideThisBanner() {
        Assert.assertFalse("Cookie exists. ", browserSteps.cookieExists(COOKIE_OPTIONS_COOKIE_NAME))
    }

    @Then("^a local cookie is created with expiry date$")
    fun cookieIsCreatedThatWouldHideThisBanner() {
        Assert.assertTrue("Cookie exists. ", browserSteps.cookieExists(COOKIE_OPTIONS_COOKIE_NAME))
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
