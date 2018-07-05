package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/appointments/booking")
class AvailableAppointmentsPage : AppointmentSharedElementsPage() {

    @FindBy(xpath = "//select[@id='type']")
    private lateinit var appointmentTypeFilter: WebElementFacade

    @FindBy(xpath = "//select[@id='location']")
    private lateinit var locationFilter: WebElementFacade

    @FindBy(xpath = "//select[@id='clinician']")
    private lateinit var clinicianFilter: WebElementFacade

    @FindBy(xpath = "//select[@id='time-period']")
    private lateinit var timePeriodFilter: WebElementFacade

    @FindBy(xpath = "//*[@id = 'appointment-date']")
    private lateinit var firstAppointmentDate: WebElementFacade

    @FindBy(xpath = "//*[@id='error-type']")
    private lateinit var typeInLineError: WebElementFacade

    @FindBy(xpath = "//*[@id='error-location']")
    private lateinit var locationInLineError: WebElementFacade

    @FindBy(xpath = "//*[@id='error-slot']")
    private lateinit var slotInLineError: WebElementFacade

    private val appointmentSlotDateXpath = "//form/span/h5[text() = '%s']"
    private val appointmentSlotTimeXpath = "$appointmentSlotDateXpath/../ul/li[contains(., '%s')]"

    fun isTypeFilterPresent(): Boolean {
        return appointmentTypeFilter.isPresent
    }

    fun isLocationsFilterPresent(): Boolean {
        return locationFilter.isPresent
    }

    fun isCliniciansFilterPresent(): Boolean {
        return clinicianFilter.isPresent
    }

    fun isTimePeriodFilterPresent(): Boolean {
        return timePeriodFilter.isPresent
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
        return appointmentTypeFilter.selectedVisibleTextValue.trim()
    }

    fun getLocationFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(locationFilter)
    }

    fun getSelectedLocation(): String {
        return locationFilter.selectedVisibleTextValue.trim()
    }

    fun getClinicianFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(clinicianFilter)
    }

    fun getSelectedClinician(): String {
        return clinicianFilter.selectedVisibleTextValue.trim()
    }

    fun getTimePeriodFilterContents(): ArrayList<String>  {
        return filterContentsAsStrings(timePeriodFilter)
    }

    fun getSelectedTimePeriod(): String {
        return timePeriodFilter.selectedVisibleTextValue.trim()
    }

    fun areAnySlotsPresent(): Boolean {
        return firstAppointmentDate.isPresent
    }

    fun selectAnAppointmentType() {
        appointmentTypeFilter.selectByIndex<WebElementFacade>(1)
    }

    fun selectAppointmentTypeByText(text: String) {
        appointmentTypeFilter.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectALocation() {
        locationFilter.selectByIndex<WebElementFacade>(1)
    }

    fun selectLocationByText(text: String) {
        locationFilter.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectClinicianByText(text: String) {
        clinicianFilter.selectByVisibleText<WebElementFacade>(text)
    }

    fun selectTimePeriodByText(text: String) {
        timePeriodFilter.selectByVisibleText<WebElementFacade>(text)
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

    fun clickOnSlotByDateAndTime(expectedDateHeading: String?, expectedTimeOnSlot: String?) {
        getTimeSlotElement(expectedDateHeading, expectedTimeOnSlot).click()
    }

    fun getInlineTypeValidationError(): String {
        return typeInLineError.text
    }

    fun getInlineLocationValidationError(): String {
        return locationInLineError.text
    }

    fun getInlineSlotValidationError(): String {
        return slotInLineError.text
    }

    fun getErrorSummaryAtRow(rowNumber: Int): String {
        return findByXpath(errorMessage.element, "//p[$rowNumber]").text
    }

    private fun getTimeSlotElement(expectedDateHeading: String?, expectedTimeOnSlot: String?) =
            findByXpath(String.format(appointmentSlotTimeXpath, expectedDateHeading, expectedTimeOnSlot!!.toLowerCase()))

    private fun filterContentsAsStrings(filter: WebElementFacade): ArrayList<String> {
        val optionElements = findAllByXpath(filter, "//option")
        val optionsAsStrings = arrayListOf<String>()
        for (option in optionElements) {
            optionsAsStrings.add(option.text.trim())
        }
        return optionsAsStrings
    }

    private fun timeSlotAtPosition(position: Int) = findByXpath("//form//li[$position]")
}
