package features.redirector.stepDefinitions

import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import config.Config
import cucumber.api.java.en.Then
import java.net.URL

class RedirectorStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps

    @When("I navigate to the redirector page with a url of '(.*)'$")
    fun iNavigateToRedirectPageWith(queryString: String) {
        val fullUrl = Config.instance.url + queryString
        browser.browseTo(fullUrl)
    }

    @Then("I am navigated to the third party site with the base url of '(.*)'$")
    fun assertNavigatedTo(url: String) {
        val host = URL(url).host
        browser.shouldHaveUrl(host)
    }

    @Then("I am navigated to a third party site")
    fun assertNoLongerOnNhsAppSite() {
        browser.shouldNotHaveUrl(Config.instance.url)
    }
}
