package pages.appointments

import models.Slot
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments")
class MyAppointmentsPage : AppointmentSharedElementsPage() {

    val bookButton = HybridPageElement(
            browserLocator = "//*[@id='book-appointments-button']",
            androidLocator = null,
            iOSLocator = "//*[@id='book-appointments-button']",
            page = this
    )

    private val successMessage = HybridPageElement(
            browserLocator = "//*[@data-purpose='success']",
            androidLocator = null,
            page = this
    )

    private val actualNoUpcomingText = HybridPageElement(
            browserLocator = "//*[@data-purpose='info']",
            androidLocator = null,
            page = this
    )

    private val cancelAppointmentLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'Cancel appointment')]",
            androidLocator = null,
            page = this
    )

    private val cannotCancelAppointmentText = HybridPageElement(
            browserLocator = "//*[contains(text(),'To cancel this appointment, contact your GP surgery.')]",
            androidLocator = null,
            page = this
    )

    private val upcomingAppointmentParentXpath = "//div[@data-purpose='appointments']"

    fun getSuccessMessage(): String = successMessage.element.text

    fun getNoUpcomingText(): String = actualNoUpcomingText.element.text

    fun getDateTimestampsOfSlots(): List<Long> {
        return getDateTimestampsOfSlots(upcomingAppointmentParentXpath)
    }

    fun getAllSlots(areCliniciansExpected: Boolean = false): ArrayList<Slot> {
        return getAllSlots(upcomingAppointmentParentXpath, areCliniciansExpected)
    }

    fun clickFirstCancelAppointmentLink() {
        cancelAppointmentLink.click()
    }

    fun getNumberOfCancelLinks(): Int {
        return cancelAppointmentLink.elements.size
    }

    fun getNumberOfAppointmentsThatCannotBeCancelled(): Int {
        return cannotCancelAppointmentText.elements.size
    }
}
