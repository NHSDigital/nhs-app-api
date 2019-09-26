package features.sharedSteps

import cucumber.api.java.en.When
import net.thucydides.core.annotations.Steps

class NavigationStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    @When("I navigate to the (.*) page for mobile devices")
    fun iNavigateToThePageForMobileDevices(pageName: String) {
        val url = PageUrl().getPageWithMobileSource(pageName)
        browser.browseTo(url)
    }

    @When("I navigate to the (.*) page for desktop")
    fun iNavigateToThePageForDesktop(pageName: String) {
        val url = PageUrl().getPageWithoutSource(pageName)
        browser.browseTo(url)
    }
}
