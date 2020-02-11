package features.more.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import features.sharedSteps.PageUrl
import mocking.MockingClient
import net.thucydides.core.annotations.Steps
import pages.MorePage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import pages.assertElementNotPresent
import pages.isDisplayed

class MoreStepDefinitions {

    @Steps
    lateinit var pageUrl: PageUrl
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var organDonationSteps: OrganDonationStepDefinitions

    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader
    lateinit var morePage: MorePage

    val mockingClient = MockingClient.instance

    @When("^I click the Messages link on the More page")
    fun iClickTheMessagesLinkOnTheMorePage() {
        morePage.btnMessages.click()
    }

    @When("^I choose to set my organ donation preferences")
    fun setOrganDonationPreferences() {
        morePage.btnOrganDonation.click()
    }

    @When("^I choose to set my data sharing preferences")
    fun setDataSharingPreferences() {
        mockingClient.forNdop {
            postTokenToNdop()
                    .respondWithNdopMockPage()
        }
        val urlForPage = pageUrl.getPage("data sharing")
        browser.browseTo(urlForPage)
    }

    @Then("^I am on the More Page$")
    fun iAmOnTheMorePage() {
        pageHeaderVisible()
        if(headerNative.onMobile()){
            moreButtonOnNavBarIsHighlighted()
        }
        morePage.assertLinksPresent()
    }

    @Then("^I see more button on the nav bar is highlighted$")
    fun moreButtonOnNavBarIsHighlighted() {
        nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

    @Then("^I see the more page header$")
    fun pageHeaderVisible() {
        webHeader.getPageTitle().withText("More")
    }

    @Then("the link to Messages is not available on the More page")
    fun theLinkToMessagesIsNotAvailableOnTheMorePage() {
        morePage.btnMessages.assertElementNotPresent()
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

    @Then("I see and can follow links including online consultation links within the more page body")
    fun iSeeAndCanFollowLinksIncludingOnlineConsultationsWithinTheMorePageBody() {
        val linksToFollow = arrayListOf(
                { followRequestGPHelpLink() },
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
            webHeader.getPageTitle().withText("Find out why your data matters")
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

    private fun followRequestGPHelpLink() {
        morePage.btnRequestGpHelp.isDisplayed
    }

    private fun navigateBackToMorePage() {
        if(headerNative.onMobile()) {
            nav.select(NavBarNative.NavBarType.MORE)
            iAmOnTheMorePage()
        }
    }
}
