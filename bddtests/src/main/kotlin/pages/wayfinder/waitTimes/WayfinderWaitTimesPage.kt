package pages.wayfinder.waitTimes

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
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

    var content = LinksWithDescriptionsContent(
        linkBlockTitle = "Wayfinder Help links",
        containerXPath = "//div",
        linkStyling = "h2")
        .addLink(helpLinkTitle, "")

    val links by lazy { LinksElement(this, content) }

    val clickableHelpLink by lazy {
        links.link(helpLinkTitle)
    }

    fun assertWayfinderWaitTimesTitleIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun assertHelpLinkIsDisplayed(){
        helpLink.assertIsVisible()
    }
}
