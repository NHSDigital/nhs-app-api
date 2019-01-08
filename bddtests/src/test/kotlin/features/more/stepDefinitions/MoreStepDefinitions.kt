package features.more.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.LocatorMethods
import pages.MorePage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative

class MoreStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var organDonationSteps: OrganDonationStepDefinitions

    lateinit var headerNative: HeaderNative

    lateinit var morePage: MorePage

    lateinit var locatorMethods: LocatorMethods

    val mockingClient = MockingClient.instance


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
        morePage.btnDataSharing.click()
        morePage.locatorMethods.waitForNativeStepToComplete()
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

        Assert.assertEquals("Test Setup Incorrect. Expected Number of links does not match those to follow. " +
                "This test must be updated if a link is added or removed.",
                morePage.expectedLinks.count(),
                linksToFollow.count())

        linksToFollow.forEachIndexed { index, link ->
            link.invoke()
            if (index != linksToFollow.size - 1)
                navigateBackToMorePage()
        }
    }

    private fun followDataSharingLink() {
        morePage.btnDataSharing.click()
        morePage.locatorMethods.waitForNativeStepToComplete()
        headerNative.waitForPageHeaderText("Find out why your data matters")
        nav.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

    private fun followOrganDonationLink() {
        morePage.btnOrganDonation.click()
        organDonationSteps.iAmOnTheOrganDonationPage()
    }

    private fun navigateBackToMorePage() {
        nav.select(NavBarNative.NavBarType.MORE)
        iAmOnTheMorePage()
    }
}
