package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.RadioButtons

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/prescription-type/")
open class TypeOfPrescriptionsPage : HybridPageObject() {
    private val repeatPrescriptionRadioButtonLabel = "A repeat prescription"
    private val nonRepeatPrescriptionRadioButtonLabel = "A non-repeat prescription"

    private val radioButtons by lazy { RadioButtons.create(this) }

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"What type of prescription do you want to order?\")]",
        page = this,
        helpfulName = "Type of Prescriptions Title"
    )

    private val continueButton = HybridPageElement(
        webDesktopLocator = "//button[@id='continue-button']",
        page = this,
        helpfulName = "Continue button"
    )

    fun assertIsDisplayed() {
        pageTitle.assertIsVisible()
    }

    fun chooseNonRepeatPrescriptionAndContinue() {
        val nonRepeatPrescriptionRadio = radioButtons.button(nonRepeatPrescriptionRadioButtonLabel)
        nonRepeatPrescriptionRadio.select()
        continueButton.click()
    }

    fun chooseRepeatPrescriptionAndContinue() {
        val repeatPrescriptionRadio = radioButtons.button(repeatPrescriptionRadioButtonLabel)
        repeatPrescriptionRadio.select()
        continueButton.click()
    }
}
