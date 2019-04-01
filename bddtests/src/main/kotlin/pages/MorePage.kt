package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.MenuLinks
import pages.sharedElements.MenuLinksContent

@DefaultUrl("http://web.local.bitraft.io:3000/more")
open class MorePage : HybridPageObject() {

    private val organDonationTitle = "Manage organ donation decision"
    private val organDonationDescription = "Help save thousands of lives in the UK every year by signing up" +
            " to become a donor on the NHS Organ Donor Register."
    private val dataSharingTitle = "Find out why your data matters"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
                    "information and choose whether or not it can be used for research and planning."

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"More\")]",
            androidLocator = null,
            page = this,
            helpfulName = "More Title"
    )

    var content = MenuLinksContent(
            title = "More",
            links = arrayOf(
                    Pair(organDonationTitle, organDonationDescription),
                    Pair(dataSharingTitle, dataSharingDescription)),
            containerXPath = "//div[@id='mainDiv']")

    val links by lazy { MenuLinks(this, content) }

    val btnOrganDonation by lazy { links.link(organDonationTitle) }
    val btnDataSharing by lazy { links.link(dataSharingTitle) }

    fun assertLinksPresent() {
        links.assertLinksPresent()
    }

    fun assertTitleIsVisible() {
        pageTitle.assertIsVisible()
    }
}
