package pages.patientPracticeMessaging

import mocking.emis.patientPracticeMessaging.Recipient
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.navigation.WebHeader
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement
import pages.text

class PatientPracticeMessagingRecipientsPage: HybridPageObject() {

    private lateinit var header: WebHeader

    private val expectedHeaderText = "Select who to message"
    private val expectedInfoText = "This is who your GP surgery lets you message. " +
            "Your message may be read by any member of staff."

    private val infoText = HybridPageElement(
            "//p[@id='infoRecipients']",
            page = this,
            helpfulName = "Information about recipients"
    )

    fun assertIsDisplayed() {
        Assert.assertEquals(
                "Header should contain text $expectedHeaderText",
                expectedHeaderText,
                header.getPageTitle().text)
    }

    fun assertInfoText() {
        infoText.waitForElement().assertIsVisible()
        Assert.assertEquals(
                "Expected recipient into text not found",
                expectedInfoText,
                infoText.text
        )
    }

    fun clickRecipient(expectedRecipients: List<Recipient>) {
        recipientLink(expectedRecipients[0].name!!).click()
    }

    fun assertRecipients(expectedRecipients: List<Recipient>) {
        expectedRecipients.forEach {
            recipientLink(it.name!!).waitForElement().assertIsVisible()
        }
    }

    private fun recipientLink(recipientName: String): HybridPageElement {
        val linkContent = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//ul[@id='recipientsMenuList']"
        ).addLink(recipientName)
        val linkElement by lazy { LinksElement(this, linkContent) }

        return linkElement.link(recipientName)
    }
}