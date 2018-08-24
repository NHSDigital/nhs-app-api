package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement

@DefaultUrl("http://localhost:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    private val byIdXpath = "//*[@id='%s']"
    private val inlineErrorByIdXpath = "$byIdXpath//*[@data-purpose='error']"
    private val guidanceParentXpath = "//*[@data-purpose='info-msg']"
    private val guidanceIconXpath = "$guidanceParentXpath//*[@data-purpose='icon'][contains(text(), '%s')]"

    private val appointmentSlotGuidance = HybridPageElement(
            browserLocator = guidanceParentXpath,
            androidLocator = "",
            page = this
    )

    private val appointmentSlotGuidanceExpand = HybridPageElement(
            browserLocator = String.format(guidanceIconXpath, "+"),
            androidLocator = "",
            page = this
    )

    private val appointmentSlotGuidanceCollapse = HybridPageElement(
            // Note that the character is a true minus sign and not a hyphen
            browserLocator = String.format(guidanceIconXpath, "−"),
            androidLocator = "",
            page = this
    )

    private val appointmentSlotGuidanceLabel = HybridPageElement(
            browserLocator = "$guidanceParentXpath//label",
            androidLocator = "",
            page = this
    )

    private val appointmentSlotGuidanceContent = HybridPageElement(
            browserLocator = "$guidanceParentXpath//*[@data-purpose='info-content']",
            androidLocator = "",
            page = this
    )

    private val appointmentTypeFilter = HybridPageElement(
            browserLocator = String.format(byIdXpath, "type"),
            androidLocator = "",
            page = this
    )

    private val locationFilter = HybridPageElement(
            browserLocator = String.format(byIdXpath, "location"),
            androidLocator = "",
            page = this
    )

    private val clinicianFilter = HybridPageElement(
            browserLocator = String.format(byIdXpath, "clinician"),
            androidLocator = "",
            page = this
    )

    private val timePeriodFilter = HybridPageElement(
            browserLocator = String.format(byIdXpath, "time-period"),
            androidLocator = "",
            page = this
    )

    private val firstAppointmentDate = HybridPageElement(
            browserLocator = "//form/span/*",
            androidLocator = "",
            page = this
    )

    private val typeInLineError = HybridPageElement(
            browserLocator = String.format(inlineErrorByIdXpath, "error-type"),
            androidLocator = "",
            page = this
    )

    private val locationInLineError = HybridPageElement(
            browserLocator = String.format(inlineErrorByIdXpath, "error-location"),
            androidLocator = "",
            page = this
    )

    private val slotInLineError = HybridPageElement(
            browserLocator = String.format(inlineErrorByIdXpath, "error-slot"),
            androidLocator = "",
            page = this
    )

    private fun timeSlotAtPosition(position: Int) = HybridPageElement(
            browserLocator = "//form//li[$position]",
            androidLocator = "",
            page = this
    )

    private fun timeSlot(date: String, time: String) = HybridPageElement(
            browserLocator = "//form/div/span[h2 = '$date']/ul/li['$time']",
            androidLocator = "",
            page = this
    )

    private val appointmentSlotDateXpath = "//form//h2[text() = '%s']"

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

    fun selectSlotByPositionNumber(position: Int) {
        return timeSlotAtPosition(position).element.click()
    }

    fun selectSlot(date: String, time: String) {
        return timeSlot(date, time).element.click()
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

    fun areAnySlotsPresent(): Boolean {
        return firstAppointmentDate.elements.isNotEmpty()
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

    fun isDateHeadingPresent(expectedDateHeading: String?): Boolean {
        return try {
            findByXpath(String.format(appointmentSlotDateXpath, expectedDateHeading)).isPresent
        } catch (e: Exception) {
            false
        }
    }

    fun isTimeSlotPresent(expectedDateHeading: String, expectedTimeOnSlot: String): Boolean {
        return try {
            timeSlot(expectedDateHeading, expectedTimeOnSlot).element.isPresent
        } catch (e: Exception) {
            false
        }
    }

    fun numberOfTimeSlotsPresent(expectedDateHeading: String, expectedTimeOnSlot: String): Int {
        return if (isTimeSlotPresent(expectedDateHeading, expectedTimeOnSlot)) {
            timeSlot(expectedDateHeading, expectedTimeOnSlot).elements.size
        } else {
            0
        }
    }

    fun isTimeSlotAtPositionSelected(position: Int): Boolean {
        return try {
            timeSlotAtPosition(position).element.getAttribute("aria-label") == "selected-slot"
        } catch (e: Exception) {
            false
        }
    }

    fun getInlineTypeValidationError(): String {
        return typeInLineError.element.text
    }

    fun getInlineLocationValidationError(): String {
        return locationInLineError.element.text
    }

    fun getInlineSlotValidationError(): String {
        return slotInLineError.element.text
    }

    fun getErrorSummarySubHeading(): String {
        return errorBanner.subHeading
    }

    fun getErrorSummaryBodyAtRow(rowNumber: Int): String {
        return errorBanner.bodyElements[rowNumber - 1]
    }

    fun isGuidancePresent(): Boolean {
        return try {
            appointmentSlotGuidance.element.isPresent
        } catch (e: Exception) {
            false
        }
    }

    fun isGuidanceContentVisible(): Boolean {
        return try {
            appointmentSlotGuidanceContent.element.isVisible
        } catch (e: Exception) {
            false
        }
    }

    fun getGuidanceContent(): String? {
        return appointmentSlotGuidanceContent.element.text
    }

    fun getGuidanceLabelText(): String? {
        return appointmentSlotGuidanceLabel.element.text
    }

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

