package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertTrue
import pages.HybridPageElement

@Suppress("TooManyFunctions")
@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    private val byIdXpathFormat = "//*[@id='%s']"
    private val guidanceParentXpath = "//*[@data-purpose='info-msg']"
    private val guidanceIconXpathFormat = "$guidanceParentXpath//*[@data-purpose='icon']$containsTextXpathSubstring"
    private val dateHeadingXpath = "//form//h2"
    private val dateHeadingByTextXpathFormat = "$dateHeadingXpath$containsTextXpathSubstring"
    private val timeSlotXpathFormat = "//form//h2%s/following-sibling::ul/li%s"
    private val timeSlotByDateAndTimeXpath = String.format(timeSlotXpathFormat, containsTextXpathSubstring, containsTextXpathSubstring)
    private val timeSlotsXpath = String.format(timeSlotXpathFormat, "", "")
    private val noAppointmentsAvailableForDateTextByDateXpathFormat = "$dateHeadingByTextXpathFormat/following-sibling::p"

    private val appointmentSlotGuidance = HybridPageElement(
            browserLocator = guidanceParentXpath,
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Slot Guidance section. "
    )

    private val appointmentSlotGuidanceExpand = HybridPageElement(
            browserLocator = String.format(guidanceIconXpathFormat, "+"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Slot Guidance expand icon. "
    )

    private val appointmentSlotGuidanceCollapse = HybridPageElement(
            // Note that the character is a true minus sign and not a hyphen
            browserLocator = String.format(guidanceIconXpathFormat, "−"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Slot Guidance collapse icon. "
    )

    private val appointmentSlotGuidanceLabel = HybridPageElement(
            browserLocator = "$guidanceParentXpath//h2",
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Slot Guidance section label. "
    )

    private val appointmentSlotGuidanceContent = HybridPageElement(
            browserLocator = "$guidanceParentXpath//*[@data-purpose='info-content']",
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Slot Guidance content. "
    )

    private val appointmentTypeFilter = HybridPageElement(
            browserLocator = String.format(byIdXpathFormat, "type"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Type filter. "
    )

    private val locationFilter = HybridPageElement(
            browserLocator = String.format(byIdXpathFormat, "location"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Location filter. "
    )

    private val clinicianFilter = HybridPageElement(
            browserLocator = String.format(byIdXpathFormat, "clinician"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment Clinician filter. "
    )

    private val timePeriodFilter = HybridPageElement(
            browserLocator = String.format(byIdXpathFormat, "time-period"),
            androidLocator = "",
            page = this,
            helpfulName = "Appointment time period filter. "
    )

    private fun timeSlotForDateAndTime(date: String, time: String) = HybridPageElement(
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

    fun isTypeFilterPresent(): Boolean {
        return appointmentTypeFilter.elements.isNotEmpty()
    }

    fun isLocationsFilterPresent(): Boolean {
        return locationFilter.elements.isNotEmpty()
    }

    fun isCliniciansFilterPresent(): Boolean {
        return clinicianFilter.elements.isNotEmpty()
    }

    fun isTimePeriodFilterPresent(): Boolean {
        return timePeriodFilter.elements.isNotEmpty()
    }

    fun selectSlot(date: String, time: String) {
        timeSlotForDateAndTime(date, time).assertIsVisible().element.click()
    }

    fun getAppointmentTypeFilterContents(): ArrayList<String> {
        return filterContentsAsStrings(appointmentTypeFilter)
    }

    fun getSelectedAppointmentType(): String {
        return appointmentTypeFilter.element.selectedVisibleTextValue.trim()
    }

    fun getLocationFilterContents(): ArrayList<String> {
        return filterContentsAsStrings(locationFilter)
    }

    fun getSelectedLocation(): String {
        return locationFilter.element.selectedVisibleTextValue.trim()
    }

    fun getClinicianFilterContents(): ArrayList<String> {
        return filterContentsAsStrings(clinicianFilter)
    }

    fun getSelectedClinician(): String {
        return clinicianFilter.element.selectedVisibleTextValue.trim()
    }

    fun getTimePeriodFilterContents(): ArrayList<String> {
        return filterContentsAsStrings(timePeriodFilter)
    }

    fun getSelectedTimePeriod(): String {
        return timePeriodFilter.element.selectedVisibleTextValue.trim()
    }

    fun getAreAnySlotsPresent(): Boolean {
        return timeSlots.elements.isNotEmpty()
    }

    fun selectAnAppointmentType() {
        appointmentTypeFilter.element.selectByIndex<WebElementFacade>(1)
    }

    fun selectAppointmentTypeByText(text: String) {
        appointmentTypeFilter.element.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectALocation() {
        locationFilter.element.selectByIndex<WebElementFacade>(1)
    }

    fun selectLocationByText(text: String) {
        locationFilter.element.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectClinicianByText(text: String) {
        clinicianFilter.element.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectTimePeriodByText(text: String) {
        timePeriodFilter.element.selectByVisibleText<WebElementFacade>(text)
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

    fun assertTimeSlotPresent(expectedDateHeading: String, expectedTimeOnSlot: String) {
        assertTrue("No timeslot present. ", timeSlotForDateAndTime(expectedDateHeading, expectedTimeOnSlot).element.isPresent)
    }

    fun numberOfTimeSlotsPresentForSpecificTime(expectedDateHeading: String, expectedTimeOnSlot: String): Int {
        timeSlotForDateAndTime(expectedDateHeading, expectedTimeOnSlot).assertSingleElementPresent()
        return timeSlotForDateAndTime(expectedDateHeading, expectedTimeOnSlot).elements.size
    }

    fun getNumberOfTimeSlotsPresent(): Int {
        assertTrue(
                "No time slots are present. Please use assertElementNotPresent() if this is correct behaviour. ",
                timeSlots.element.isPresent
        )
        return timeSlots.elements.size
    }

    fun assertGuidancePresent() {
        appointmentSlotGuidance.assertSingleElementPresent()
    }

    fun assertGuidanceNotPresent() {
        appointmentSlotGuidance.assertElementNotPresent()
    }

    fun assertGuidanceContentNotVisible() {
        appointmentSlotGuidanceContent.assertElementNotPresent()
    }

    fun getGuidanceContent(): String = appointmentSlotGuidanceContent.element.text

    fun getGuidanceLabelText(): String = appointmentSlotGuidanceLabel.element.text

    fun expandGuidance() {
        appointmentSlotGuidanceExpand.element.click()
    }

    fun collapseGuidance() {
        appointmentSlotGuidanceCollapse.element.click()
    }

    private fun filterContentsAsStrings(filter: HybridPageElement): ArrayList<String> {
        val optionElements = findAllByXpath(filter.element, "//option")
        val optionsAsStrings = arrayListOf<String>()
        for (option in optionElements) {
            optionsAsStrings.add(option.text.trim())
        }
        return optionsAsStrings
    }
}

