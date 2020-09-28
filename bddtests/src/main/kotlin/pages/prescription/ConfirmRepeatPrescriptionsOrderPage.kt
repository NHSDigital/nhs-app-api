package pages.prescription

import models.prescriptions.MedicationCourse
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.nominatedPharmacy.PharmacyDetailComponent
import pages.HybridPageElement
import pages.HybridPageObject
import pages.asciiText
import pages.sendKeys

const val HEADER_RETRIES = 20
@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/confirm-prescription-details")
open class ConfirmRepeatPrescriptionsOrderPage : HybridPageObject() {
    val title by lazy {
        HybridPageElement(
                "//h1[normalize-space(text())='Confirm prescription']",
                "//h1[normalize-space(text())='Confirm prescription']",
                null,
                null,
                this,
                helpfulName = "header")
    }
    val specialRequestTextXPath = "//*[@id='specialRequestText']"

    val pharmacyDetailComponent = PharmacyDetailComponent()

    val confirmAndOrderRepeatPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_confirm_and_order_prescription']",
            androidLocator = null,
            page = this
    )

    val changeThisPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'Change this prescription')]",
            androidLocator = "//button[contains(text(), 'Change this prescription')]",
            iOSLocator = "//button[contains(text(), 'Change this prescription')]",
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

    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmAndOrderRepeatPrescriptionButton.sendEnterKey()
    }

    fun clickChangeThisPrescriptionButton() {
        changeThisPrescriptionButton.sendKeys(Keys.ENTER)
    }

    fun errorSendingOrderErrorIsVisible(): Boolean {
        return findByXpath("//*[contains(text(), \"$serverErrorHeader\")]").isVisible
    }
}
