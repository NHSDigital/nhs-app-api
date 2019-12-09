package features.sharedSteps

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import net.thucydides.core.annotations.Steps

class NavigationStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    @Given("^I am using the native app user agent$")
    fun iAmUsingTheNativeAppUserAgent() {
        browser.setUserAgentSource("ios")
    }

    @When("^I navigate to the (.*) page$")
    fun iNavigateToThePage(pageName: String) {
        val url = PageUrl().getPage(pageName)
        browser.browseTo(url)
    }
}
