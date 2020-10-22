package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksWithDescriptionsContent

@DefaultUrl("http://web.local.bitraft.io:3000/more")
open class MorePage : HybridPageObject() {

    private val organDonationTitle = "Manage your organ donation decision"
    private val organDonationDescription = "Help save thousands of lives in the UK every year by signing up" +
            " to become a donor on the NHS Organ Donor Register"

    private val dataSharingTitle = "Choose if data from your health records is shared for research and planning"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
                    "information and choose whether or not it can be used for research and planning"

    private val messagesTitle = "Messages"
    private val messagesDescription = "View messages from health services and the NHS App"

    private val appMessagesTitle = "Health information and updates"
    private val appMessagesDescription = "View messages from health services and the NHS App"

    val content = LinksWithDescriptionsContent(
            linkBlockTitle = "More",
            containerXPath = "//div[@id='mainDiv']",
            linkStyling = "h2")
            .addLink(organDonationTitle, organDonationDescription)
            .addLink(dataSharingTitle, dataSharingDescription)

    val links by lazy { LinksElement(this, content) }

    val btnOrganDonation by lazy { links.link(organDonationTitle, organDonationDescription) }
    val btnMessages by lazy { links.link(messagesTitle, messagesDescription) }
    val btnAppMessages by lazy { links.link(appMessagesTitle, appMessagesDescription) }
    val btnDataSharing by lazy { links.link(dataSharingTitle, dataSharingDescription) }

    private val unreadIndicator = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_messages_discIndicator']",
            androidLocator = null,
            page = this)

    fun assertUnreadIndicatorVisible() {
        unreadIndicator.assertIsVisible()
    }

    fun assertProxyText(patientName: String){
        val expectedText = arrayListOf(
                "It's not possible to access this section while acting on $patientName's behalf.",
                "Switch to your profile to access this section.")
        HybridPageElement(
                webDesktopLocator = "//div[p[@id='shutter-summary-text']]", page=this)
                .actOnTheElement {
                    val actualText = it.findElements<WebElement>(By.xpath("./p"))
                            .map { element -> element.text.trim() }
                    Assert.assertEquals("proxy text", expectedText, actualText)
                }
    }
}
