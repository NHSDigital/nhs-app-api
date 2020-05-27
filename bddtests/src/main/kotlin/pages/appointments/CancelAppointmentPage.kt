package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.isPresent
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/appointments")
class CancelAppointmentPage : AppointmentSharedElementsPage() {
    val contactToCancelTitle = "Contact your GP surgery to cancel"
    val cannotCancelTitle = "Sorry, you cannot cancel this appointment"
    val alreadyCancelled = "This may be because the appointment is already cancelled or it occurs in the past."
    val serviceProblemTitle = "Sorry, there is a problem with the service - Appointments"
    val contactGpToCancelHeader = "Contact your GP surgery to cancel"
    val cannotCancelRightNow = "You cannot cancel appointments online right now. " +
            "Call your GP surgery as soon as possible to let them know you need to cancel."
    val tooLateToCancel = "It's too late to cancel this appointment online. Call your GP surgery as soon " +
            "as possible to let them know you need to cancel."

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

    override val titleText: String = "Cancel GP appointment"

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
