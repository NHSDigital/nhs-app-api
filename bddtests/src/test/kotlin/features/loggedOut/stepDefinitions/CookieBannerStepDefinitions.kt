package features.loggedOut.stepDefinitions

import config.Config
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.loggedOut.steps.CookieBannerSteps
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.CheckMySymptomsPage
import pages.loggedOut.CookieBanner
import pages.loggedOut.LoginPage

private const val COOKIE_OPTIONS_COOKIE_NAME = "nhso.cookie_options"
private const val LOGIN_PAGE = "login"
private const val CHECK_YOUR_SYMPTOMS_PAGE = "check your symptoms"
private const val DO_SEE = "see"
private const val DO_NOT_SEE = "do not see"
private const val WILL = "will"
private const val WILL_NOT = "will not"

class CookieBannerStepDefinitions {

    @Steps
    private lateinit var browserSteps: BrowserSteps
    @Steps
    lateinit var cookieBannerSteps: CookieBannerSteps

    private lateinit var loginPage: LoginPage
    private lateinit var checkMySymptomsPage: CheckMySymptomsPage
    private lateinit var cookieBanner: CookieBanner

    @When("^I am on the ($LOGIN_PAGE|$CHECK_YOUR_SYMPTOMS_PAGE) logged-out page$")
    fun iAmOnTheLoggedOutPage(page: String) {
        when (page) {
            LOGIN_PAGE -> browserSteps.goToApp()
            CHECK_YOUR_SYMPTOMS_PAGE -> {
                browserSteps.goToApp()
                loginPage.symptomsButton.click()
                checkMySymptomsPage.isConditionsHeaderVisible()
                checkMySymptomsPage.isNhs111HeaderVisible()
            }
        }
    }

    @When("^I select the information link$")
    fun iSelectTheInformationLink() {
        cookieBanner.cookiesInformationLink.click()
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
