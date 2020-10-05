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

    private val dataSharingTitle = "Find out why your data matters"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
                    "information and choose whether or not it can be used for research and planning"

    private val messagesTitle = "Messages"
    private val messagesDescription = "Get messages from your GP surgery and other NHS services."

    private val appMessagesTitle = "Health information and updates"
    private val appMessagesDescription = "View messages from health services and the NHS App"

    private val olcAdminHelpTitle = "Additional GP services"
    private val olcAdminHelpDescription = "Get sick notes and GP letters or ask about recent tests"

    private val engageAdminTitle = "Additional GP services"
    private val engageAdminDescription = "Get sick notes and GP letters or ask about recent tests"

    private val sharedLinksTitle = "Shared links"
    private val sharedLinksDescription = "View links your doctor or health professional has shared with you"

    private val pkbCieSharedHealthLinksTitle = "Shared health links"
    private val pkbCieSharedHealthLinksDescription =
            "View links or documents your health team has shared with you, or add your own"

    val content = LinksWithDescriptionsContent(
            linkBlockTitle = "More",
            containerXPath = "//div[@id='mainDiv']",
            linkStyling = "h2")
            .addLink(organDonationTitle, organDonationDescription)
            .addLink(dataSharingTitle, dataSharingDescription)
            .addLink(olcAdminHelpTitle, olcAdminHelpDescription)

    val links by lazy { LinksElement(this, content) }

    val btnOrganDonation by lazy { links.link(organDonationTitle, organDonationDescription) }
    val btnMessages by lazy { links.link(messagesTitle, messagesDescription) }
    val btnAppMessages by lazy { links.link(appMessagesTitle, appMessagesDescription) }
    val btnDataSharing by lazy { links.link(dataSharingTitle, dataSharingDescription) }
    val btnOlcAdminHelp by lazy { links.link(olcAdminHelpTitle, olcAdminHelpDescription) }
    val btnEngageAdmin by lazy { links.link(engageAdminTitle, engageAdminDescription) }
    val btnSharedLinks by lazy { links.link(sharedLinksTitle, sharedLinksDescription)}
    val btnPkbCieSharedHealthLinks by lazy {
        links.link(pkbCieSharedHealthLinksTitle, pkbCieSharedHealthLinksDescription)}

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
