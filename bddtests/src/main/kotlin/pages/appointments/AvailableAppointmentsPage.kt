package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.sharedElements.DropdownElement
import pages.sharedElements.ExpandElement
import pages.withoutRetrying

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {
    private val dateHeadingXpath = "//*[@data-purpose='appointment-day-heading']"
    private val dateHeadingByTextXpathFormat = "$dateHeadingXpath$containsTextXpathSubstring"
    private val timeSlotXpathFormat = "$dateHeadingXpath%s" +
            "/ancestor::summary/following-sibling::div//*$appointmentTimeXpath%s" +
            "/ancestor::a[@data-purpose='confirm-timeslot']"
    private val timeSlotByDateAndTimeXpath = String.format(
            timeSlotXpathFormat,
            containsTextXpathSubstring,
            containsTextXpathSubstring
    )
    private val timeSlotsXpath = String.format(timeSlotXpathFormat, "", "", "")
    
    private val noAppointmentsForFilterWarningContent = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='no-appointments-matching-filter']",
            page = this
    )

    override val titleText: String = "Book an appointment"

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
            "Practice member (optional)",
            "Appointment Clinician filter",
            this
    )

    val timePeriodFilter = DropdownElement(
            "Filter available appointments",
            "Appointment time period filter",
            this
    )

    fun assertNoAppointmentSlotsAvailableForCurrentFilterWarningIsVisible() {
        noAppointmentsForFilterWarningContent.assertSingleElementPresent().assertIsVisible()

        noAppointmentsForFilterWarningContent.actOnTheElement {

            val bannerTitle = it.findElement<WebElement>(By.xpath("./h2[1]")).text
            Assert.assertEquals(
                    "Expected h2 title",
                    "No appointments available for your search", bannerTitle)

            val actualText = it.findElements<WebElement>(By.tagName("p"))
                    .map { element -> element.text }

            val expectedText = arrayListOf(
                    "You can choose different filter options, or select \"No preference\" for the practice member, " +
                            "to show any available appointments.",
                    "If you cannot find the appointment you need, contact your GP surgery.",
                    "For urgent medical advice, go to 111.nhs.uk or call 111.")

            val message = "Expected text. " +
                    "Expected: ${expectedText.joinToString()}. " +
                    "Actual: ${actualText.joinToString()}."
            Assert.assertEquals(message, expectedText.count(), actualText.count())
            Assert.assertTrue(message, expectedText.containsAll(actualText))
        }
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

    private val desktopBackLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='main-back-button']",
            page = this
    )

    fun clickDesktopBackButton() {
        desktopBackLink.click()
    }

    fun selectSlot(date: String, time: String, sessionName: String?) {
        dateHeadingByText(date)
                .assertSingleElementPresent()
                .assertIsVisible()
                .click()

        timeSlotForDateTimeSession(date, time, sessionName)
                .assertSingleElementPresent()
                .assertIsVisible()
                .click()
    }

    fun getAreAnySlotsPresent(): Boolean {
        return timeSlots.withoutRetrying().elements.isNotEmpty()
    }

    fun assertDateHeadingPresent(expectedDateHeading: String) {
        dateHeadingByText(expectedDateHeading).assertSingleElementPresent()
    }

    fun getNumberOfDateHeadingsPresent(): Int {
        return dateHeading.elements.size
    }

    fun getNumberOfTimeSlotsPresent(): Int {
        return timeSlots.elements.size
    }
}
