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
            " to become a donor on the NHS Organ Donor Register."
    private val dataSharingTitle = "Find out why your data matters"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
                    "information and choose whether or not it can be used for research and planning."
    private val messagesTitle = "Your messages"
    private val messagesDescription = "Get messages from your GP surgery and other NHS services."

    // online consultations menu item
    private val requestGpHelpTitle = "Additional GP services"
    private val requestGpHelpDescription = "Get sick notes and GP letters or ask about recent tests."

    private val messagesAndConsultationsTitle = "Messages and online consultations"
    private val messagesAndConsultationsDescription = "Message your healthcare team, " +
            "or answer questions online and get a response from a health professional"

    val content = LinksWithDescriptionsContent(
            linkBlockTitle = "More",
            containerXPath = "//div[@id='mainDiv']",
            linkStyling = "h2")
            .addLink(organDonationTitle, organDonationDescription)
            .addLink(dataSharingTitle, dataSharingDescription)
            .addLink(requestGpHelpTitle, requestGpHelpDescription)

    val links by lazy { LinksElement(this, content) }

    val btnOrganDonation by lazy { links.link(organDonationTitle, organDonationDescription) }
    val btnMessages by lazy { links.link(messagesTitle, messagesDescription) }
    val btnDataSharing by lazy { links.link(dataSharingTitle, dataSharingDescription) }
    val btnRequestGpHelp by lazy { links.link(requestGpHelpTitle, requestGpHelpDescription) }
    val btnMessagesAndConsultations by lazy { links.link(messagesAndConsultationsTitle,
            messagesAndConsultationsDescription) }

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
