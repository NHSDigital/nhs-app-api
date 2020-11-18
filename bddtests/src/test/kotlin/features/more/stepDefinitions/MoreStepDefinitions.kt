package features.more.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.MorePage
import pages.navigation.HeaderNative
import pages.navigation.WebHeader
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail

class MoreStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps

    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader
    lateinit var morePage: MorePage

    @When("^I click the Messages link on the More page")
    fun iClickTheMessagesLinkOnTheMorePage() {
        morePage.btnMessages.click()
    }

    @When("^I see the unread indicator on the More page")
    fun iSeeTheUnreadIndicatorOnTheMorePage() {
        morePage.assertUnreadIndicatorVisible()
    }

    @Then("^I am on the More page$")
    fun iAmOnTheMorePage() {
        webHeader.getPageTitle().waitForElement().withText("More")
    }

    @Then("^the More page explains that it is not possible to access it while acting on behalf of someone else$")
    fun theMorePageExplainsThatItIsNotPossibleToAccessItWhileActingOnBehalfOfSomeoneElse(){
        morePage.assertProxyText(LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.getOrFail())
    }
}
