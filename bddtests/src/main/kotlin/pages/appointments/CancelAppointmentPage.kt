package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/appointments")
class CancelAppointmentPage : AppointmentSharedElementsPage() {
    val contactToCancelTitle = "Cannot cancel appointment"
    val alreadyCancelled = "This may be because the appointment is already cancelled or it occurs in the past."
    val tooLateToCancel = "The appointment cannot be cancelled in the NHS App because it's within an hour."
    val contactGpToCancel = "Contact your GP surgery as soon as possible to let them know you need to cancel."

    private val checkDetailsText = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='info']/p",
        page = this
    )

    private val dropDownMenuLabel = HybridPageElement(
        webDesktopLocator = "//label[@for='txt_reason']",
        page = this
    )

    val dropDownMenu = HybridPageElement(
        webDesktopLocator = "//select[@id='txt_reason']",
        page = this,
        helpfulName = "dropdown menu"
    )

    override val titleText: String = "Cancel GP appointment"

    fun getCheckDetailsText(): String {
        return checkDetailsText.text
    }

    fun getReasonDropDownLabelText(): String {
        return dropDownMenuLabel.text
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
