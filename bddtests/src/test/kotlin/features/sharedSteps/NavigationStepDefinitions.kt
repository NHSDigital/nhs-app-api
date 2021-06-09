package features.sharedSteps

import config.Config
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.navigation.NavBarNative
import utils.GlobalSerenityHelpers
import utils.set

class NavigationStepDefinitions {
    @Steps
    private lateinit var browser: BrowserSteps

    @Steps
    lateinit var navSteps: NavigationSteps

    @Given("^I am using the native app user agent$")
    fun iAmUsingTheNativeAppUserAgent() {
        browser.setUserAgentSource("ios")

        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidNativeRedirectUri)
        GlobalSerenityHelpers.GP_SESSION_REDIRECT_URI.set(Config.instance.cidNativeGpSessionRedirectUri)
    }

    @When("^I navigate to the (.*) page$")
    fun iNavigateToThePage(pageName: String) {
        val url = PageUrl.getRelativePagePath(pageName)
        browser.browseTo(url)
    }

    @When("^I browse directly to '(.*)' in the NHS App$")
    fun iBrowseDirectlyToThePage(path: String) {
        browser.browseViaHttpGet(path)
    }

    @When("^I navigate to (\\w+)$")
    fun iNavigateTo(tab: String) {
        navSteps.select(NavBarNative.NavBarType.valueOf(tab.toUpperCase()))
    }

    @Then("^I am redirected to '(.*)'$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }
}
