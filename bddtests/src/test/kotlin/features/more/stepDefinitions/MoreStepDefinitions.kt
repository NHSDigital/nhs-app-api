package features.more.stepDefinitions

import config.Config
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.MorePage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import java.net.URL

class MoreStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps

    lateinit var headerNative: HeaderNative

    lateinit var morePage: MorePage
    val mockingClient = MockingClient.instance


    @When("^I choose to set my organ donation preferences")
    fun setOrganDonationPreferences() {
        morePage.btnOrganDonation.element.click()
    }

    @When("^I choose to set my data sharing preferences")
    fun setDataSharingPreferences() {
        mockingClient.forNdop {
            postTokenToNdop()
                    .respondWithNdopMockPage()
        }
        morePage.btnDataSharing.element.click()
    }

    @Then("^I am on the More Page$")
    fun iAmOnTheMorePage() {
        pageHeaderVisible()
        moreButtonOnNavBarIsHighlighted()
        morePage.assertLinksPresent()
    }

    @Then("^I see more button on the nav bar is highlighted$")
    fun moreButtonOnNavBarIsHighlighted() {
        nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

    @Then("^I see the more page header$")
    fun pageHeaderVisible() {
        if(morePage.onMobile()){
            headerNative.getPageTitle("More").nativeElement.isDisplayed
        }
        headerNative.assertIsVisible("More")
    }

    @Then("I see and can follow links within the more page body$")
    fun iSeeAndCanFollowLinksWithinTheMorePageBody() {
        morePage.assertLinksPresent()
        val linksToFollow = arrayListOf(
                { followDataSharingLink() },
                { followOrganDonationLink() }
        )

        Assert.assertEquals("Test Setup Incorrect. Expected Number of links does not match those to follow. This test must be updated if a link is added or removed.",
                morePage.expectedLinks.count(),
                linksToFollow.count())

        linksToFollow.forEachIndexed { index, link ->
            link.invoke()
            if (index != linksToFollow.size - 1)
                navigateBackToMorePage()
        }
    }

    private fun followDataSharingLink() {
        morePage.btnDataSharing.element.click()
        headerNative.waitForPageHeaderText("Sharing health data preferences")
        nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }


    fun followOrganDonationLink() {
        morePage.btnOrganDonation.element.click()
        if (morePage.onMobile()){
            URL(Config.instance.organDonation)
        } else {
            browser.changeTab(URL(Config.instance.organDonation))
            browser.shouldHaveUrl(Config.instance.organDonation)
        }
    }

    private fun navigateBackToMorePage() {
        nav.select(NavBarNative.NavBarType.MORE)
        iAmOnTheMorePage()
    }
}
