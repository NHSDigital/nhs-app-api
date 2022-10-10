package pages.wayfinder.waitTimes

import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.annotations.NotImplementedException
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertElementNotPresent
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/hospital-referrals-appointments/waiting-lists")
open class WayfinderWaitTimesPage : HybridPageObject() {

    private val helpLinkTitle = "What to do if something is missing, incorrect " +
            "or has not been changed or cancelled"

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Waiting lists\")]",
        page = this,
        helpfulName = "Waiting lists - h1"
    )

    private val helpLink = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),\"What to do if something is missing, incorrect or " +
                "has not been changed or cancelled\")]",
        page = this,
        helpfulName = "help link - h2"
    )

    private val waitTimeZeroHeaderText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(), \"You're on 0 waiting lists\")]",
        page = this
    )

    private val waitTimeOneHeaderText = HybridPageElement(
        webDesktopLocator = "//p[contains(text(), \"You're on a waiting list\")]",
        page = this
    )

    private val waitTimeComponentCardHeader = HybridPageElement(
        webDesktopLocator = "//h3[contains(text(),\"Waiting list\")]",
        page = this,
        helpfulName = "Wait time card header - h3"
    )

    var content = LinksWithDescriptionsContent(
        linkBlockTitle = "Wayfinder Help links",
        containerXPath = "//div",
        linkStyling = "h2")
        .addLink(helpLinkTitle, "")

    val links by lazy { LinksElement(this, content) }

    val clickableHelpLink by lazy {
        links.link(helpLinkTitle)
    }

    private val errorPageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"Cannot view waiting times\")]",
        page = this,
        helpfulName = "Cannot view waiting times - h1"
    )

    private val errorPageTechnicalProblem = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"There is a technical problem.\")]",
        page = this,
        helpfulName = "There is a technical problem. - p"
    )

    private val errorPageWaitingTimesUnavailable = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"Information on waiting times is currently unavailable.\")]",
        page = this,
        helpfulName = "Information on waiting times is currently unavailable. - p"
    )

    private val errorPageTryAgainLater = HybridPageElement(
        webDesktopLocator = "//p[contains(text(),\"Try again later.\")]",
        page = this,
        helpfulName = "Try again later. - p"
    )

    fun assertWayfinderWaitTimesTitleIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun assertHelpLinkIsDisplayed() {
        helpLink.assertIsVisible()
    }

    fun assertWayfinderWaitTimesComponentDisplayed(numberOfWaitTimes: Int) {
        if (numberOfWaitTimes == 0) {
            waitTimeZeroHeaderText.assertIsVisible()
            waitTimeComponentCardHeader.assertElementNotPresent()
        }
        else if (numberOfWaitTimes == 1) {
            waitTimeOneHeaderText.assertIsVisible()
            waitTimeComponentCardHeader.assertIsVisible()
        }
        else {
            throw NotImplementedException("More than one Wait Time Component assertion not implemented")
        }
    }

    fun assertWayfinderWaitTimesErrorPageTitleIsDisplayed() {
        errorPageTitle.assertIsVisible()
    }

    fun assertWayfinderWaitTimesErrorPageDisplayed() {
        errorPageTechnicalProblem.assertIsVisible()
        errorPageWaitingTimesUnavailable.assertIsVisible()
        errorPageTryAgainLater.assertIsVisible()
    }
}
