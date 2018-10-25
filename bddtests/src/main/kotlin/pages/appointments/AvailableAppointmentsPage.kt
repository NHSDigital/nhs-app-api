package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertTrue
import org.openqa.selenium.NoSuchElementException
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    private val dateHeadingXpath = "//form//h2"
    private val dateHeadingByTextXpathFormat = "$dateHeadingXpath$containsTextXpathSubstring"
    private val timeSlotXpathFormat = "//form//h2%s/following-sibling::ul/li%s"
    private val timeSlotByDateAndTimeXpath = String.format(timeSlotXpathFormat,
                                                           containsTextXpathSubstring,
                                                           containsTextXpathSubstring)
    private val timeSlotsXpath = String.format(timeSlotXpathFormat, "", "")
    private val noAppointmentsAvailableForDateTextByDateXpathFormat =
            "$dateHeadingByTextXpathFormat/following-sibling::p"

    val guidance = AppointmentGuidancePageObject(this)

    val appointmentTypeFilter = AvailableAppointmentFilter (
            "type",
             "Appointment Type filter",
            this
    )

    val locationFilter = AvailableAppointmentFilter (
            "location",
            "Appointment Location filter",
            this
    )

    val clinicianFilter = AvailableAppointmentFilter (
            "clinician",
            "Appointment Clinician filter",
            this
    )

    val timePeriodFilter =AvailableAppointmentFilter (
            "time-period",
            "Appointment time period filter",
            this
    )

    val warningMessage = HybridPageElement(
            browserLocator = "//div[@data-purpose='warning']",
            androidLocator = null,
            page = this
    )

    fun timeSlotForDateAndTime(date: String, time: String) = HybridPageElement(
            browserLocator = String.format(timeSlotByDateAndTimeXpath, date, time),
            androidLocator = "",
            page = this,
            helpfulName = "Time slot by date and time. "
    )

    private val timeSlots = HybridPageElement(
            browserLocator = timeSlotsXpath,
            androidLocator = "",
            page = this,
            helpfulName = "Any time slot. "
    )

    private val dateHeading = HybridPageElement(
            browserLocator = dateHeadingXpath,
            androidLocator = "",
            page = this,
            helpfulName = "Any date heading. "
    )

    private fun dateHeadingByText(date: String) = HybridPageElement(
            browserLocator = String.format(dateHeadingByTextXpathFormat, date),
            androidLocator = "",
            page = this,
            helpfulName = "Date heading by text. "
    )

    private fun noAppointmentsAvailableForDateTextByDate(date: String) = HybridPageElement(
            browserLocator = String.format(noAppointmentsAvailableForDateTextByDateXpathFormat, date),
            androidLocator = "",
            page = this,
            helpfulName = "Text displayed when there are no appointments on a particular date. "
    )

    fun selectSlot(date: String, time: String) {
        val slot = timeSlotForDateAndTime(date, time)
                .assertSingleElementPresent()
                .assertIsVisible()
                .scrollToElement()
        slot.element.click()
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