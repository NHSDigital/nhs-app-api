package features.more.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.MorePage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import pages.onlineConsultations.OnlineConsultationsUnavailablePage
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail

class MoreStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var organDonationSteps: OrganDonationStepDefinitions

    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader
    lateinit var morePage: MorePage
    lateinit var onlineConsultationsUnavailablePage: OnlineConsultationsUnavailablePage

    @When("^I click the Messages link on the More page")
    fun iClickTheMessagesLinkOnTheMorePage() {
        morePage.btnMessages.click()
    }

    @When("^I see the unread indicator on the More page")
    fun iSeeTheUnreadIndicatorOnTheMorePage() {
        morePage.assertUnreadIndicatorVisible()
    }

    @When("^I click the Data Sharing link on the More page")
    fun iClickTheDataSharingLinkOnTheMorePage() {
        morePage.btnDataSharing.click()
    }

    @When("^I choose to set my organ donation preferences")
    fun setOrganDonationPreferences() {
        morePage.btnOrganDonation.click()
    }

    @Then("^I am on the More Page$")
    fun iAmOnTheMorePage() {
        pageHeaderVisible()
        if(headerNative.onMobile()){
            moreButtonOnNavBarIsHighlighted()
        }
    }

    @Then("^I see more button on the nav bar is highlighted$")
    fun moreButtonOnNavBarIsHighlighted() {
        nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

    @Then("^I see the more page header$")
    fun pageHeaderVisible() {
        webHeader.getPageTitle().waitForElement().withText("More")
    }

    @Then("^the More page explains that it is not possible to access it while acting on behalf of someone else$")
    fun theMorePageExplainsThatItIsNotPossibleToAccessItWhileActingOnBehalfOfSomeoneElse(){
        morePage.assertProxyText(LinkedProfilesSerenityHelpers.PROXY_DISPLAY_NAME.getOrFail())
    }

    @Then("I see and can follow links within the more page body$")
    fun iSeeAndCanFollowLinksWithinTheMorePageBody() {
        val linksToFollow = arrayListOf(
                { followDataSharingLink() },
                { followOrganDonationLink() }
        )

        linksToFollow.forEachIndexed { index, link ->
            link.invoke()
            if (index != linksToFollow.size - 1 && headerNative.onMobile())
                navigateBackToMorePage()
        }
    }

    private fun followDataSharingLink() {
        if (headerNative.onMobile()) {
            morePage.btnDataSharing.click()
            morePage.locatorMethods.waitForNativeStepToComplete()
            webHeader.getPageTitle().withText(
                    "Choose if data from your health records is shared for research and planning")
            nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
        } else {
            browser.storeCurrentTabCount()
            morePage.btnDataSharing.click()
            browser.assertNewTab()
        }
    }

    private fun followOrganDonationLink() {
        if (headerNative.onMobile()){
            morePage.btnOrganDonation.click()
            organDonationSteps.iAmOnTheOrganDonationPage()
        } else{
            browser.storeCurrentTabCount()
            morePage.btnOrganDonation.click()
            browser.assertNewTab()
        }
    }

    private fun navigateBackToMorePage() {
        if(headerNative.onMobile()) {
            nav.select(NavBarNative.NavBarType.MORE)
            iAmOnTheMorePage()
        }
    }
}
