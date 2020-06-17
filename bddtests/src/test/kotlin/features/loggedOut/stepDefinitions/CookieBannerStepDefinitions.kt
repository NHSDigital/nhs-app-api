package features.loggedOut.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.loggedOut.steps.CookieBannerSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.CookieSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertElementNotPresent
import pages.assertIsNotVisible
import pages.assertIsVisible
import pages.loggedOut.CookieBanner
import utils.GlobalSerenityHelpers
import utils.getOrNull

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
    private lateinit var cookieBannerSteps: CookieBannerSteps

    private lateinit var cookieBanner: CookieBanner

    @Given("^session storage is cleared$")
    fun sessionStorageIsCleared() {
        cookieSteps.clearSessionStorage("hasClosedCookies")
        browserSteps.refreshPage()
    }

    @When("^I am on the login logged-out page$")
    fun iAmOnTheLoginLoggedOutPageForTheFirstTime() {
        browserSteps.goToApp()
        if (GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.getOrNull<Boolean>() == true) {
            cookieSteps.setInstructionsCookie("true")
        }
    }

    @When("^I am on the login logged-out page for the first time$")
    fun iAmOnTheLoginLoggedOutPage() {
        browserSteps.goToApp()
        cookieSteps.verifyCookieDoesntExist("SkipPreRegistrationPage")
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
        cookieBanner.cookieWrapper.assertIsNotVisible()
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
            browserSteps.browseViaHttpGet(url)
            when (visibility) {
                WILL -> cookieBanner.cookieBannerText1.assertIsVisible()
                WILL_NOT -> cookieBanner.cookieBannerText1.assertElementNotPresent()
            }
        }
    }
}
