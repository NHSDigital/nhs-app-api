package pages.patientPracticeMessaging

import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.RadioButtons

class PatientPracticeMessagingUrgencyPage: HybridPageObject() {
    private val urgentRadioButtonLabel = "Yes, I need advice now"
    private val nonUrgentRadioButtonLabel = "No, my message is not urgent"

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
}