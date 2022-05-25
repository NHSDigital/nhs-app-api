package pages.patientPracticeMessaging

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.sharedElements.RadioButtons
import pages.text

class PatientPracticeMessagingUrgencyPage: HybridPageObject() {
    private val expectedHeaderText = "Do you need urgent advice?"
    private val urgentRadioButtonLabel = "Yes, I need advice now"
    private val nonUrgentRadioButtonLabel = "No, my message is not urgent"

    private lateinit var header: WebHeader

    private val radioButtons by lazy { RadioButtons.create(this) }

    private val continueButton = HybridPageElement(
            "//button[@id='continueButton']",
            page = this,
            helpfulName = "Continue button")

    fun assertIsDisplayed() {
        Assert.assertEquals(
                "Header should contain text $expectedHeaderText",
                expectedHeaderText,
                header.getPageTitle().text)
    }

    fun chooseUrgentAndContinue() {
        val urgentRadio = radioButtons.button(urgentRadioButtonLabel)
        urgentRadio.select()
        continueButton.click()
    }

    fun clickContinue() {
        continueButton.click()
    }

    fun chooseNonUrgentAndContinue() {
        val nonUrgentRadio = radioButtons.button(nonUrgentRadioButtonLabel)
        nonUrgentRadio.select()
        continueButton.click()
    }

    fun assertNoRecipientsHeader() {
        Assert.assertEquals(
                "Header should contain text 'Cannot send GP surgery messages'",
                "Cannot send GP surgery messages",
                header.getPageTitle().text)
    }

    fun assertGpMessagesNotTurnedOnMessage() {
        val messagesNotTurnedOn = HybridPageElement(
            "//div[@id='noRecipients']//p[@id='messagesNotTurnedOn']",
            page=this
        )
        Assert.assertEquals("At the moment, your GP Surgery has not turned on messaging in the NHS App.",
            messagesNotTurnedOn.text)
    }

    fun assertContactYourGpMessage() {
        val contactYourGp = HybridPageElement(
            "//div[@id='noRecipients']//p[@id='conactYourGp']",
            page=this
        )
        Assert.assertEquals("Contact your GP for more information or to access GP services.",
            contactYourGp.text)
    }

    fun assertForUrgentMedicalAdviceMessage() {
        val forUrgentMedicalAdvice = HybridPageElement(
                "//div[@id='noRecipients']//p[@id='subHeader']",
                page=this
        )
        Assert.assertEquals("For urgent medical advice, go to 111.nhs.uk or call 111.",
            forUrgentMedicalAdvice.text)
    }
}
