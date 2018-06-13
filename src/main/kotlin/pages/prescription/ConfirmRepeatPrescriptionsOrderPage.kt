package pages.prescription

import mocking.emis.models.MedicationCourse
import net.serenitybdd.core.annotations.findby.By
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.pages.WrongPageError
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageObject
import pages.HybridPageObject.Companion.PageType
import pages.navigation.Header

@DefaultUrl("http://localhost:3000/prescriptions/confirm-prescription-details")
open class ConfirmRepeatPrescriptionsOrderPage : HybridPageObject(PageType.WEBVIEW_APP) {
    var headerText: String = "Select medication"
    lateinit var headerBar: Header

    @FindBy(how = How.ID, using = "btn_confirm_and_order_prescription")
    private lateinit var confirmAndOrderRepeatPrescriptionButton: WebElementFacade

    @FindBy(how = How.XPATH, using = "//button[contains(text(), 'Change this prescription')]")
    private lateinit var changeThisPrescriptionButton: WebElementFacade

    val serverErrorPageTitle = "Error Sending Prescription"
    val serverErrorPageHeader = "Error sending request"
    val serverErrorHeader = "Sorry, there's been a problem sending your request"
    val serverErrorSubHeader = "Please go back and try again."
    val serverErrorMessage = "If the problem persists and you need to order a repeat prescription now, please contact your GP surgery directly."
    val serverErrorRetryButtonText = "Back to my repeat prescriptions"

    override fun shouldBeDisplayed() {
        if(!headerBar.isVisible(headerText)) {
            throw WrongPageError("The expected header is not visible, you are on the wrong page.")
        }

        super.shouldBeDisplayed()
    }

    fun verifySelectedRepeatPrescriptions(selectedCourses: List<MedicationCourse>) {
        val repeatPrescriptions = findAllByXpath("//div[@data-purpose='selected-prescription']")

        Assert.assertEquals(selectedCourses.size, repeatPrescriptions.size)

        for (i in selectedCourses.indices) {
            var expectedCourse = selectedCourses[i]
            var currentCourseOnScreen = repeatPrescriptions[i]

            var nameOnScreen = currentCourseOnScreen.findElement(By.xpath( "./label[@data-purpose='prescription-name']"))
            var instructionsOnScreen = currentCourseOnScreen.findElement(By.xpath("./p[@data-purpose='prescription-description']"))

            Assert.assertEquals(expectedCourse.name, nameOnScreen.text)
            Assert.assertEquals(expectedCourse.getInstructionsText(), instructionsOnScreen.text)
        }
    }

    fun clickConfirmAndOrderRepeatPrescriptionButton() {
        confirmAndOrderRepeatPrescriptionButton.sendKeys(Keys.ENTER)
    }

    fun clickChangeThisPrescriptionButton() {
        changeThisPrescriptionButton.sendKeys(Keys.ENTER)
    }
}
