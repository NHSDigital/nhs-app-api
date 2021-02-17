package pages.prescription

import models.prescriptions.MedicationCourse
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.asciiText
import pages.nominatedPharmacy.PharmacyDetailComponent

const val HEADER_RETRIES = 20
@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/confirm-prescription-details")
open class ConfirmRepeatPrescriptionsOrderPage : HybridPageObject() {
    val title by lazy {
        HybridPageElement(
                "//h1[normalize-space(text())='Check your prescription details before you order']",
                "//h1[normalize-space(text())='Check your prescription details before you order']",
                null,
                null,
                this,
                helpfulName = "header")
    }
    val specialRequestTextXPath = "//*[@id='specialRequestText']"

    val nominatedPharmacyTextXPath = "//*[@id='my-nominated-pharmacy']"

    val pharmacyDetailComponent = PharmacyDetailComponent()

    val confirmAndOrderRepeatPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_confirm_and_order_prescription']",
            androidLocator = null,
            page = this
    )

    val changeThisPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//a[@id='changeRepeatPrescription']",
            page = this
    )

    val changeThisSpecialRequest = HybridPageElement(
            webDesktopLocator = "//a[@id='changeSpecialRequest']",
            page = this
    )

    val serverErrorPageHeader = "Error sending order"
    val serverErrorHeader = "There's been a problem sending your order"
    val serverErrorMessage = "Go back and try again. If the problem continues and you need to order" +
            " a repeat prescription now, contact your GP surgery directly. " +
            "For urgent medical advice, go to 111.nhs.uk or call 111."
    val serverErrorRetryButtonText = "Go to your prescriptions"

    override fun shouldBeDisplayed() {
        title.waitForElement(HEADER_RETRIES)
        super.shouldBeDisplayed()
    }

    fun verifySelectedRepeatPrescriptions(selectedCourses: List<MedicationCourse>) {
        val repeatPrescriptions = findAllByXpath("//div[@data-purpose='selected-prescription']")

        Assert.assertEquals(selectedCourses.size, repeatPrescriptions.size)

        for (i in selectedCourses.indices) {
            val expectedCourse = selectedCourses[i]
            val currentCourseOnScreen = repeatPrescriptions[i]

            val nameOnScreen = currentCourseOnScreen.findBy<WebElementFacade>(
                    "[data-purpose='prescription-name']")
            val instructionsOnScreen = currentCourseOnScreen.findBy<WebElementFacade>(
                    "[data-purpose='prescription-description']").asciiText

            Assert.assertEquals(expectedCourse.name, nameOnScreen.text)
            Assert.assertEquals(expectedCourse.getInstructionsText(), instructionsOnScreen)
        }
    }

    fun specialRequestElementIsVisible(): Boolean {
        return findByXpath(specialRequestTextXPath).isVisible
    }

    fun getSpecialRequest(): String {
        return findByXpath(specialRequestTextXPath).text
    }

    fun getNominatedPharmacy(): String {
        return findByXpath(nominatedPharmacyTextXPath).text
    }

    fun nominatedPharmacyIsVisible(): Boolean {
        return findByXpath(nominatedPharmacyTextXPath).isVisible
    }

    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmAndOrderRepeatPrescriptionButton.sendEnterKey()
    }

    fun clickChangeThisPrescriptionButton() {
        changeThisPrescriptionButton.click()
    }

    fun clickChangeThisSpecialRequest() {
        changeThisPrescriptionButton.click()
    }

    fun errorSendingOrderErrorIsVisible(): Boolean {
        return findByXpath("//*[contains(text(), \"$serverErrorHeader\")]").isVisible
    }
}
