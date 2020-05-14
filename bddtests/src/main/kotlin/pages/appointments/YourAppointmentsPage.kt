package pages.appointments

import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.assertElementNotPresent
import pages.text
import pages.withoutRetrying

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/gp-appointments")
class YourAppointmentsPage : AppointmentSharedElementsPage() {

    val upcomingAppointmentsHeading = HybridPageElement(
            webDesktopLocator = "//h2[text()='Upcoming appointments']",
            page = this
    )

    val pastAppointmentsHeading = HybridPageElement(
            webDesktopLocator = "//h2[normalize-space(text())='Past appointments']",
            page = this
    )

    val bookButton = HybridPageElement(
            webDesktopLocator = "//*[@id='book-appointments-button']",
            androidLocator = "//*[@id='book-appointments-button']",
            iOSLocator = "//*[@id='book-appointments-button']",
            page = this
    )

    private val actualNoUpcomingText = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='upcoming-info']",
            page = this
    )

    private val actualNoPastText = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='past-info']",
            page = this
    )

    private val cancelAppointmentLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'Cancel this appointment')]",
            page = this
    )

    private val cannotCancelAppointmentText = HybridPageElement(
            webDesktopLocator = "//*[contains(text(),'To cancel this appointment, contact your GP surgery.')]",
            page = this
    )

    private val backLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),\"Go to your appointments\")]",
            page = this
    )

    override val titleText: String = "Your GP appointments"

    private val upcomingAppointmentParentXpath = "//div[@data-purpose='upcoming-appointments']"
    private val historicalAppointmentParentXpath = "//div[@data-purpose='past-appointments']"

    fun getNoUpcomingText(): String = actualNoUpcomingText.text

    fun getNoPastText(): String = actualNoPastText.text

    fun assertPastTextNotPresent() = actualNoPastText.assertElementNotPresent()

    fun getDateTimestampsOfUpcomingSlots(): List<Long> {
        return getDateTimestampsOfSlots(upcomingAppointmentParentXpath)
    }

    fun getDateTimestampsOfHistoricalSlots(): List<Long> {
        return getDateTimestampsOfSlots(historicalAppointmentParentXpath)
    }

    fun getAllUpcomingSlots(areCliniciansExpected: Boolean = false,
                            isTelephoneAppointment: Boolean = false): ArrayList<Slot> {
        return getAllSlots(upcomingAppointmentParentXpath, areCliniciansExpected, isTelephoneAppointment)
    }

    fun getAllHistoricalSlots(areCliniciansExpected: Boolean = false,
                              isTelephoneAppointment: Boolean =  false): ArrayList<Slot> {
        return getAllSlots(historicalAppointmentParentXpath, areCliniciansExpected, isTelephoneAppointment)
    }

    fun getWebAppointmentSlotDivs(): List<WebElementFacade> {
        return findAllByXpath(upcomingAppointmentParentXpath)
    }

    fun clickFirstCancelAppointmentLink() {
        cancelAppointmentLink.click()
    }

    fun getNumberOfCancelLinks(): Int {
        return cancelAppointmentLink.withoutRetrying().elements.size
    }

    fun getNumberOfAppointmentsThatCannotBeCancelled(): Int {
        return cannotCancelAppointmentText.withoutRetrying().elements.size
    }

    fun getTelephoneField(): ArrayList<String> {
        return getTelephoneSlot(upcomingAppointmentParentXpath)
    }

    fun getBackLink(): String = backLink.text
}
