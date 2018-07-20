package pages.appointments

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.ErrorPage
import pages.HybridPageElement

@DefaultUrl("http://localhost:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    private val filterXpath = "//select[@id='%s']"
    private val byIdWithinSpanXpath = "//span[@id='%s']"

    private val appointmentTypeFilter = HybridPageElement(
            browserLocator = String.format(filterXpath, "type"),
            androidLocator = "",
            page = this
    )

    private val locationFilter = HybridPageElement(
            browserLocator = String.format(filterXpath, "location"),
            androidLocator = "",
            page = this
    )

    private val clinicianFilter = HybridPageElement(
            browserLocator = String.format(filterXpath, "clinician"),
            androidLocator = "",
            page = this
    )

    private val timePeriodFilter = HybridPageElement(
            browserLocator = String.format(filterXpath, "time-period"),
            androidLocator = "",
            page = this
    )

    private val firstAppointmentDate = HybridPageElement(
            browserLocator = "//form/span/*",
            androidLocator = "",
            page = this
    )

    private val typeInLineError = HybridPageElement(
            browserLocator = String.format(byIdWithinSpanXpath, "error-type"),
            androidLocator = "",
            page = this
    )

    private val locationInLineError = HybridPageElement(
            browserLocator = String.format(byIdWithinSpanXpath, "error-location"),
            androidLocator = "",
            page = this
    )

    private val slotInLineError = HybridPageElement(
            browserLocator = String.format(byIdWithinSpanXpath, "error-slot"),
            androidLocator = "",
            page = this
    )

    val tryAgainButton = HybridPageElement(
            browserLocator = "//button",
            androidLocator = null,
            page = this
    ).withText("Try again")

    private val appointmentSlotDateXpath = "//form/span/h5[text() = '%s']"
    private val appointmentSlotTimeXpath = "$appointmentSlotDateXpath/../ul/li[contains(., '%s')]"

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

    fun selectFirstSlot() {
        return selectSlotByPositionNumber(1)
    }

    fun selectSlotByPositionNumber(position: Int) {
        return timeSlotAtPosition(position).click()
    }

    fun getAppointmentTypeFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(appointmentTypeFilter)
    }

    fun getSelectedAppointmentType(): String {
        return appointmentTypeFilter.element.selectedVisibleTextValue.trim()
    }

    fun getLocationFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(locationFilter)
    }

    fun getSelectedLocation(): String {
        return locationFilter.element.selectedVisibleTextValue.trim()
    }

    fun getClinicianFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(clinicianFilter)
    }

    fun getSelectedClinician(): String {
        return clinicianFilter.element.selectedVisibleTextValue.trim()
    }

    fun getTimePeriodFilterContents(): ArrayList<String>  {
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

    fun isTimeSlotPresent(expectedDateHeading: String?, expectedTimeOnSlot: String?): Boolean {
        return try {
            getTimeSlotElement(expectedDateHeading, expectedTimeOnSlot).isPresent
        } catch (e: Exception) {
            false
        }
    }

    fun isTimeSlotAtPositionSelected(position: Int): Boolean {
        return try {
            timeSlotAtPosition(position).getAttribute("aria-label") == "selected-slot"
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

    fun getErrorSummaryAtRow(rowNumber: Int): String {
        val error = switchToPage(ErrorPage::class.java)
        return error.paragraph(rowNumber).element.text
    }

    private fun getTimeSlotElement(expectedDateHeading: String?, expectedTimeOnSlot: String?) =
            findByXpath(String.format(appointmentSlotTimeXpath, expectedDateHeading, expectedTimeOnSlot!!.toLowerCase()))

    private fun filterContentsAsStrings(filter: HybridPageElement): ArrayList<String> {
        val optionElements = findAllByXpath(filter.element, "//option")
        val optionsAsStrings = arrayListOf<String>()
        for (option in optionElements) {
            optionsAsStrings.add(option.text.trim())
        }
        return optionsAsStrings
    }

    private fun timeSlotAtPosition(position: Int) = findByXpath("//form//li[$position]")
}
