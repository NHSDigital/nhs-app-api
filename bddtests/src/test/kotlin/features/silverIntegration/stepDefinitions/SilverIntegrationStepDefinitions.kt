package features.silverIntegration.stepDefinitions

import io.cucumber.java.en.Then
import pages.silverIntegration.FeatureNotAvailablePage

open class SilverIntegrationStepDefinitions {

    private lateinit var featureNotAvailablePage: FeatureNotAvailablePage

    @Then("^I see silver integration error page loaded with title (.*)$")
    fun iSeeSilverIntegrationErrorPageLoadedWithTitle(title: String) {
        featureNotAvailablePage.isLoaded(title)
    }

    @Then("^I select the Go to NHS App homepage link from the feature not available page$")
    fun iSelectTheGoToNHSAppHomepageLinkFromTheFeatureNotAvailablePage() {
        featureNotAvailablePage.goToNHSAppHome.click()
    }
}
