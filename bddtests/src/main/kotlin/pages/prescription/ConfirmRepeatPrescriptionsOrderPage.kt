package pages.prescription

import models.prescriptions.MedicationCourse
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.waitForNonStaleElementToBecomeVisible

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/confirm-prescription-details")
open class ConfirmRepeatPrescriptionsOrderPage : HybridPageObject() {
    var headerText: String = "Confirm prescription"
    lateinit var headerBar: HeaderNative

    val specialRequestTextXPath = "//*[@id='specialRequestText']"

    val confirmAndOrderRepeatPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_confirm_and_order_prescription']",
            androidLocator = null,
            page = this
    )

    val changeThisPrescriptionButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Change this prescription')]",
            androidLocator = null,
            page = this
    )

    val serverErrorPageHeader = "Error sending order"
    val serverErrorHeader = "There's been a problem sending your order"
    val serverErrorMessage = "Go back and try again. If the problem continues and you need to order" +
            " a repeat prescription now, contact your GP surgery directly. For urgent medical advice, call 111."
    val serverErrorRetryButtonText = "Back to my repeat prescriptions"

    override fun shouldBeDisplayed() {
        headerBar.assertIsVisible(headerText)
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
                    "[data-purpose='prescription-description']")

            Assert.assertEquals(expectedCourse.name, nameOnScreen.text)
            Assert.assertEquals(expectedCourse.getInstructionsText(), instructionsOnScreen.text)
        }
    }

    fun specialRequestElementIsVisible(): Boolean {
        return findByXpath(specialRequestTextXPath).isVisible
    }

    fun getSpecialRequest(): String {
        return findByXpath(specialRequestTextXPath).text
    }

    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmAndOrderRepeatPrescriptionButton.waitForNonStaleElementToBecomeVisible().sendEnterKey()
    }

    fun clickChangeThisPrescriptionButton() {
        changeThisPrescriptionButton.element.sendKeys(Keys.ENTER)
    }

    fun errorSendingOrderErrorIsVisible(): Boolean {
        return findByXpath("//*[contains(text(), \"$serverErrorHeader\")]").isVisible
    }
}
