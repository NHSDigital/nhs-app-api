package pages.appointments

import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://localhost:3000/appointments")
class MyAppointmentsPage : AppointmentSharedElementsPage() {

    val bookButton = HybridPageElement(
            browserLocator = "//*[@id='btn_floating']",
            androidLocator = null,
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

    private val upcomingAppointmentParentXpath = "//div[@data-purpose='appointments']"

    fun getSuccessMessage(): String = successMessage.element.text

    fun getNoUpcomingText(): String = actualNoUpcomingText.element.text

    fun getDateTimestampsOfSlots(): List<Long> {
        return getDateTimestampsOfSlots(upcomingAppointmentParentXpath)
    }

    fun getAllSlots(areCliniciansExpected: Boolean = false): ArrayList<Slot> {
        return getAllSlots(upcomingAppointmentParentXpath, areCliniciansExpected)
    }

    fun getWebAppointmentSlotDivs(): List<WebElementFacade> {
        return findAllByXpath("$upcomingAppointmentParentXpath")
    }

    fun getSlotAtIndex(index: Int): Slot {
        return getSlotAtIndex(upcomingAppointmentParentXpath, index)
    }

    fun clickFirstCancelAppointmentLink() {
        cancelAppointmentLink.element.click()
    }

    fun getNumberOfCancelLinks(): Int {
        return cancelAppointmentLink.elements.size
    }
}
