package features.sharedSteps

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import net.thucydides.core.annotations.Steps
import utils.GlobalSerenityHelpers
import utils.set

class NavigationStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    @Given("^I am using the native app user agent$")
    fun iAmUsingTheNativeAppUserAgent() {
        browser.setUserAgentSource("ios")
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidNativeRedirectUri)
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
}
