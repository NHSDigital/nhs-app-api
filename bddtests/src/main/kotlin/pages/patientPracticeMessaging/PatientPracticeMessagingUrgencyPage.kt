package pages.patientPracticeMessaging

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.avoidChromeWebDriverServiceCrash
import pages.navigation.WebHeader
import pages.sharedElements.RadioButtons
import pages.text

class PatientPracticeMessagingUrgencyPage: HybridPageObject() {
    private val urgentRadioButtonLabel = "Yes, I need advice now"
    private val nonUrgentRadioButtonLabel = "No, my message is not urgent"

    private lateinit var header: WebHeader

    private val radioButtons by lazy { RadioButtons.create(this) }

    private val continueButton = HybridPageElement(
            "//button[@id='continueButton']",
            page = this,
            helpfulName = "Continue button")

    fun chooseUrgentAndContinue() {
        val urgentRadio = radioButtons.button(urgentRadioButtonLabel)
        urgentRadio.select()
        continueButton.click()
    }

    fun chooseNonUrgentAndContinue() {
        val nonUrgentRadio = radioButtons.button(nonUrgentRadioButtonLabel)
        nonUrgentRadio.select()
        continueButton.click()
    }

    fun assertNoRecipientsHeader() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        Assert.assertEquals(
                "Header should contain text 'You currently cannot send messages'",
                "You cannot currently send messages",
                header.getPageTitle().text)
    }

    fun assertNoRecipientsMessage() {
        val noRecipientsMessage = HybridPageElement(
                "//div[@id='noRecipients']//p",
                page=this
        )
        Assert.assertEquals("Contact your GP surgery for more information. For urgent medical advice, " +
                "go to 111.nhs.uk or call 111.",
                noRecipientsMessage.text);
    }
}
