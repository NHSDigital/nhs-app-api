package features.sharedSteps

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.navigation.WebHeader
import utils.GlobalSerenityHelpers
import utils.set
import java.lang.IllegalArgumentException

class NavigationStepDefinitions {
    @Steps
    private lateinit var browser: BrowserSteps
    private lateinit var webHeader: WebHeader

    @Given("^I am using the native app user agent$")
    fun iAmUsingTheNativeAppUserAgent() {
        browser.setUserAgentSource("ios")

        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
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
        when(tab.toUpperCase()){
            "ADVICE" -> webHeader.clickAdvicePageLink()
            "APPOINTMENTS" -> webHeader.clickAppointmentsPageLink()
            "PRESCRIPTIONS" -> webHeader.clickPrescriptionsPageLink()
            "YOUR_HEALTH" -> webHeader.clickYourHealthPageLink()
            "MESSAGES" -> webHeader.clickMessagesPageLink()
            else -> throw IllegalArgumentException("$tab not implemented")
        }
    }

    @Then("^I am redirected to '(.*)'$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }
}
