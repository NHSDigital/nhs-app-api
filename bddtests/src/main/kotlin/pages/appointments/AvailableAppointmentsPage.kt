package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertTrue
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.sharedElements.BannerObject
import pages.sharedElements.DropdownElement
import pages.sharedElements.ExpandElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    private val dateHeadingXpath = "//*[@data-purpose='appointment-day-heading']"
    private val dateHeadingByTextXpathFormat = "$dateHeadingXpath$containsTextXpathSubstring"
    private val timeSlotXpathFormat = "$dateHeadingXpath%s/following-sibling::ul/li/a" +
            "//*$appointmentTimeXpath%s/ancestor::a"
    private val timeSlotByDateAndTimeXpath = String.format(
            timeSlotXpathFormat,
            containsTextXpathSubstring,
            containsTextXpathSubstring
    )
    private val timeSlotsXpath = String.format(timeSlotXpathFormat, "", "", "")
    private val noAppointmentsAvailableForDateTextByDateXpathFormat =
            "$dateHeadingByTextXpathFormat/following-sibling::p"

    val guidance = ExpandElement(this)

    val appointmentTypeFilter = DropdownElement(
            "Type of appointment",
            "Appointment Type filter",
            this
    )

    val locationFilter = DropdownElement(
            "Location",
            "Appointment Location filter",
            this
    )

    val clinicianFilter = DropdownElement(
            "Practice member",
            "Appointment Clinician filter",
            this
    )

    val timePeriodFilter = DropdownElement(
            "Filter by date",
            "Appointment time period filter",
            this
    )

    fun warning(title: String? = null): BannerObject {
        return BannerObject.warning(this, title = title)
    }

    fun timeSlotForDateTimeSession(date: String, time: String, sessionName: String?): HybridPageElement {
        val xPathSuffixWithAppointmentSessionName = if (sessionName == null) {
            ""
        } else {
            String.format("//*$appointmentSessionNameXpath$containsTextXpathSubstring/ancestor::a", sessionName)
        }
        return HybridPageElement(
                webDesktopLocator = String.format(
                        timeSlotByDateAndTimeXpath,
                        date,
                        time
                ).plus(xPathSuffixWithAppointmentSessionName),
                androidLocator = null,
                page = this,
                helpfulName = "Time slot by date, time and session name. "
        )
    }

    private val timeSlots = HybridPageElement(
            webDesktopLocator = timeSlotsXpath,
            androidLocator = null,
            page = this,
            helpfulName = "Any time slot. "
    )

    private val dateHeading = HybridPageElement(
            webDesktopLocator = dateHeadingXpath,
            androidLocator = null,
            page = this,
            helpfulName = "Any date heading. "
    )

    private fun dateHeadingByText(date: String) = HybridPageElement(
            webDesktopLocator = String.format(dateHeadingByTextXpathFormat, date),
            androidLocator = null,
            page = this,
            helpfulName = "Date heading by text. "
    )

    private fun noAppointmentsAvailableForDateTextByDate(date: String) = HybridPageElement(
            webDesktopLocator = String.format(noAppointmentsAvailableForDateTextByDateXpathFormat, date),
            androidLocator = null,
            page = this,
            helpfulName = "Text displayed when there are no appointments on a particular date. "
    )

    val backToMyAppointmentsButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(),'Back to my appointments')]",
            androidLocator = null,
            page = this
    )

    fun selectSlot(date: String, time: String, sessionName: String?) {
        timeSlotForDateTimeSession(date, time, sessionName)
                .assertSingleElementPresent()
                .assertIsVisible()
                .click()
        waitForSpinnerToDisappear()
    }

    fun getAreAnySlotsPresent(): Boolean {
        return timeSlots.elements.isNotEmpty()
    }

    fun assertDateHeadingPresent(expectedDateHeading: String) {
        dateHeadingByText(expectedDateHeading).assertSingleElementPresent()
    }

    fun getNoSlotsAvailableTextAtDate(date: String): String {
        val hybridPageElement = noAppointmentsAvailableForDateTextByDate(date)
        hybridPageElement.assertSingleElementPresent()
        return hybridPageElement.element.text
    }

    fun getNumberOfDateHeadingsPresent(): Int {
        assertTrue(
                "No date headings are present. Please use assertElementNotPresent() if this is correct behaviour. ",
                dateHeading.element.isPresent
        )
        return dateHeading.elements.size
    }

    fun getNumberOfTimeSlotsPresent(): Int {
        assertTrue(
                "No time slots are present. Please use assertElementNotPresent() if this is correct behaviour. ",
                timeSlots.element.isPresent
        )
        return timeSlots.elements.size
    }

}