package pages.prescription

import models.prescriptions.MedicationCourse
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.pages.WrongPageError
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageObject
import pages.HybridPageObject.Companion.PageType
import pages.HybridPageElement
import pages.navigation.Header

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/confirm-prescription-details")
open class ConfirmRepeatPrescriptionsOrderPage : HybridPageObject(PageType.WEBVIEW_APP) {
    var headerText: String = "Confirm prescription"
    lateinit var headerBar: Header

    val specialRequestText = HybridPageElement(
            browserLocator = "//*[@id='specialRequestText']",
            androidLocator = null,
            page = this
    )

    val confirmAndOrderRepeatPrescriptionButton = HybridPageElement(
            browserLocator = "//*[@id='btn_confirm_and_order_prescription']",
            androidLocator = null,
            page = this
    )

    val changeThisPrescriptionButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Change this prescription')]",
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

    fun getSpecialRequest(): String {
        return specialRequestText.element.text
    }

    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmAndOrderRepeatPrescriptionButton.element.sendKeys(Keys.ENTER)
    }

    fun clickChangeThisPrescriptionButton() {
        changeThisPrescriptionButton.element.sendKeys(Keys.ENTER)
    }
}
