package pages.appointments

import constants.AppointmentDateTimeFormat.Companion.frontendDateFormat
import constants.AppointmentDateTimeFormat.Companion.frontendTimeFormat
import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.WebElement
import pages.HybridPageObject
import pages.HybridPageElement
import java.text.SimpleDateFormat
import java.util.ArrayList

open class AppointmentSharedElementsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val xPathRoot = "//*"
    private val appointmentDateXpath = "[@aria-label='date']"
    private val appointmentTimeXpath = "[@aria-label='start time']"
    private val appointmentSessionNameXpath = "[@aria-label='session name']"
    private val appointmentLocationXpath = "[@aria-label='location']"
    private val appointmentClinicianXPath = "[@aria-label='clinician %d']"

    val inLineError = HybridPageElement(
        browserLocator = "//*[@id='errorLabel']/p/span[@data-purpose='error']",
        androidLocator = null,
        page = this
    )

    val errorSummaryHeading = HybridPageElement(
            browserLocator = "//*[@data-purpose='error-heading']",
            androidLocator = null,
            page = this
    )

    val errorSummaryBody = HybridPageElement(
            browserLocator = "//*[@data-purpose='error']",
            androidLocator = null,
            page = this
    )

    fun getSelectedAppointmentDateText(): String {
        return findByXpath(xPathRoot + appointmentDateXpath).text
    }

    fun getSelectedAppointmentTimeText(): String {
        return findByXpath(xPathRoot + appointmentTimeXpath).text
    }

    fun getSelectedAppointmentSessionNameText(): String {
        return findByXpath(xPathRoot + appointmentSessionNameXpath).text
    }

    fun getSelectedAppointmentLocationText(): String {
        return findByXpath(xPathRoot + appointmentLocationXpath).text
    }

    fun getDateTimestampsOfSlots(appointmentListParentXpath: String): List<Long> {
        val slotTimestamps = arrayListOf<Long>()
        val dateTimeFormat = SimpleDateFormat("$frontendTimeFormat $frontendDateFormat")
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentListParentXpath)
        appointmentSlotDivs.forEach { slotDiv ->
            val timestamp = retrieveDateTimeFromSlotElement(slotDiv)
            slotTimestamps.add(dateTimeFormat.parse(timestamp).time)
        }
        return slotTimestamps
    }

    fun getAllSlots(appointmentListParentXpath: String): ArrayList<Slot> {
        val slotList = arrayListOf<Slot>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentListParentXpath)

        appointmentSlotDivs.forEach { slotDiv ->
            val slot = convertToSlotObject(slotDiv, isMyAppointmentSlot = true)
            slotList.add(slot)
        }
        return slotList
    }

    fun getSlotAtIndex(appointmentListParentXpath: String, index: Int): Slot {
        val appointmentSlotDiv = retrieveAppointmentSlotDivAtPosition(appointmentListParentXpath, index + 1)
        return convertToSlotObject(appointmentSlotDiv, isMyAppointmentSlot = true)
    }

    fun getSelectedAppointmentClinicianTextAtPosition(position: Int): String {
        return findByXpath(String.format(xPathRoot + appointmentClinicianXPath, position)).text
    }

    fun getInlineValidationError(): String {
        return inLineError.element.text
    }

    fun getErrorSummaryHeading(): String {
        return errorSummaryHeading.element.text
    }

    fun getErrorSummaryBody(): String {
        return errorSummaryBody.element.text
    }

    private fun convertToSlotObject(parentContainer: WebElementFacade, parentToSlotDivRelativePath: String = "", isMyAppointmentSlot: Boolean = false): Slot {
        val slot = Slot()
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) xPathRoot else "$parentToSlotDivRelativePath/"
        slot.time = findByXpath(parentContainer, relativePath + appointmentTimeXpath).text
        slot.session = findByXpath(parentContainer, relativePath + appointmentSessionNameXpath).text
        slot.date = findByXpath(parentContainer, relativePath + appointmentDateXpath).text
        slot.location = findByXpath(parentContainer, relativePath + appointmentLocationXpath).text

        retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath + appointmentClinicianXPath, isMyAppointmentSlot)
        return slot
    }

    private fun retrieveDateTimeFromSlotElement(slotElement: WebElementFacade): String {
        val time = findByXpath(slotElement, xPathRoot + appointmentTimeXpath).text
        val date = findByXpath(slotElement, xPathRoot + appointmentDateXpath).text
        return "$time $date"
    }

    private fun retrieveAppointmentSlotDivs(containerDivXpath: String): List<WebElementFacade> {
        return findAllByXpath("$containerDivXpath/div")
    }

    private fun retrieveAppointmentSlotDivAtPosition(containerDivXpath: String, index: Int): WebElementFacade {
        return findByXpath("$containerDivXpath/div[$index]")
    }

    private fun retrieveClinicianAndAddToSlot(slot: Slot, parentContainer: WebElementFacade, relativePath: String, isMyAppointmentSlot: Boolean) {
        val clinicians = when(isMyAppointmentSlot) {
            true -> findAllByXpath(parentContainer, "$relativePath//span[starts-with(@aria-label, 'clinician')]")
            false -> findAllByXpath(parentContainer, relativePath + "ul/li")
        }

        clinicians.forEach { clinician ->
            slot.clinician.add(clinician.text)
        }
    }
}
