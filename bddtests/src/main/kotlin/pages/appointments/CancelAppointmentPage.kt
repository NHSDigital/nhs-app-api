package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.isPresent
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/appointments")
class CancelAppointmentPage : AppointmentSharedElementsPage() {

    private val checkDetailsText = HybridPageElement(
            webDesktopLocator = "//div[@data-purpose='info']/p",
            androidLocator = null,
            page = this
    )

    private val dropDownMenuLabel = HybridPageElement(
            webDesktopLocator = "//label[@for='txt_reason']",
            androidLocator = null,
            page = this
    )

    private val dropDownMenu = HybridPageElement(
            webDesktopLocator = "//select[@id='txt_reason']",
            androidLocator = null,
            page = this
    )

    override val titleText: String = "Cancel appointment"

    fun getCheckDetailsText(): String {
        return checkDetailsText.text
    }

    fun getReasonDropDownLabelText(): String {
        return dropDownMenuLabel.text
    }

    fun containsDropDownMenu(): Boolean {
        return try {
            dropDownMenu.isPresent
        } catch (e: org.openqa.selenium.NoSuchElementException) {
            false
        }
    }

    fun selectReason(reason: String): Boolean {
        return try {
            dropDownMenu.actOnTheElement { it.selectByVisibleText<WebElementFacade>(reason) }
            true
        } catch (e: NoSuchElementException) {
            false
        }
    }
}
