package features.loggedOut.stepDefinitions

import features.loggedOut.steps.CookieBannerSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.CookieSteps
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.assertElementNotPresent
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
