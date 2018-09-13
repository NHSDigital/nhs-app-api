package features.more.stepDefinitions

import config.Config
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.dataSharing.steps.DataSharingSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.MorePage
import pages.navigation.Header
import pages.navigation.NavBar
import java.net.URL

class MoreStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var dataSharing: DataSharingSteps
    @Steps
    lateinit var nav: NavigationSteps

    lateinit var headerBar: Header
    lateinit var morePage: MorePage


    @When("^I choose to set my organ donation preferences")
    fun setOrganDonationPreferences() {
        morePage.btnOrganDonation.element.click()
    }

    @When("^I choose to set my data sharing preferences")
    fun setDataSharingPreferences() {
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
        nav.assertSelectedTab("MORE")
    }

    @Then("^I see the more page header$")
    fun pageHeaderVisible() {
        headerBar.assertIsVisible("More")
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
        dataSharing.assertIsDisplayed()
    }

    private fun followOrganDonationLink() {
        morePage.btnOrganDonation.element.click()
        browser.changeTab(URL(Config.instance.organDonation))
        browser.shouldHaveUrl(Config.instance.organDonation)
    }

    private fun navigateBackToMorePage() {
        nav.navBar.select(NavBar.NavBarType.MORE)
        iAmOnTheMorePage()
    }
}
