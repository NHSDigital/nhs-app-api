package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://localhost:3000/appointments")
class CancelAppointmentPage : AppointmentSharedElementsPage() {

    val checkDetailsText = HybridPageElement(
            browserLocator = "//main/div/p",
            androidLocator = null,
            page = this
    )

    val dropDownMenuLabel = HybridPageElement(
            browserLocator = "//label[@for='txt_reason']",
            androidLocator = null,
            page = this
    )

    val dropDownMenu = HybridPageElement(
            browserLocator = "//select[@id='txt_reason']",
            androidLocator = null,
            page = this
    )


    fun getCheckDetailsText(): String {
        return checkDetailsText.element.text
    }

    fun getReasonDropDownLabelText(): String {
        return dropDownMenuLabel.element.text
    }

    fun containsDropDownMenu(): Boolean {
        return try {
            dropDownMenu.element.isPresent
        } catch (e: org.openqa.selenium.NoSuchElementException) {
            false
        }
    }

    fun selectReason(reason: String): Boolean {
        return try {
            dropDownMenu.element.selectByVisibleText<WebElementFacade>(reason)
            true
        } catch (e: NoSuchElementException) {
            false
        }
    }
}
