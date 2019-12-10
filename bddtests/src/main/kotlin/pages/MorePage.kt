package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/more")
open class MorePage : HybridPageObject() {

    private val organDonationTitle = "Manage your organ donation decision"
    private val organDonationDescription = "Help save thousands of lives in the UK every year by signing up" +
            " to become a donor on the NHS Organ Donor Register."
    private val dataSharingTitle = "Find out why your data matters"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
                    "information and choose whether or not it can be used for research and planning."
    val messagesTitle = "Your messages"

    // online consultations menu item
    private val requestGpHelpTitle = "Additional GP services"
    private val requestGpHelpDescription = "Get sick notes and GP letters or ask about recent tests."

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"More\")]",
            androidLocator = null,
            page = this,
            helpfulName = "More Title"
    )

    var content = LinksWithDescriptionsContent(
            linkBlockTitle = "More",
            containerXPath = "//div[@id='mainDiv']",
            linkStyling = "h2")
            .addLink(organDonationTitle, organDonationDescription)
            .addLink(dataSharingTitle, dataSharingDescription)
            .addLink(requestGpHelpTitle, requestGpHelpDescription)

    val links by lazy { LinksElement(this, content) }

    val btnOrganDonation by lazy { links.link(organDonationTitle) }
    val btnMessages by lazy { links.link(messagesTitle) }
    val btnDataSharing by lazy { links.link(dataSharingTitle) }
    val btnRequestGpHelp by lazy { links.link(requestGpHelpTitle) }

    fun assertLinksPresent() {
        links.assertLinksPresent(true)
    }
}
