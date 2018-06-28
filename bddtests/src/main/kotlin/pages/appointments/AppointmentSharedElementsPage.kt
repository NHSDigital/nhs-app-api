package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.By
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.WebElement
import pages.HybridPageObject
import java.text.SimpleDateFormat
import java.util.ArrayList

open class AppointmentSharedElementsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    private val xPathRoot = "//*"
    private val appointmentDateXpath = "[@aria-label='date']"
    private val appointmentTimeXpath = "[@aria-label='start time']"
    private val appointmentSessionNameXpath = "[@aria-label='session name']"
    private val appointmentLocationXpath = "[@aria-label='location']"
    private val appointmentClinicianXPath = "[@aria-label='clinician %d']"

    @FindBy(xpath = "//*[@id='errorLabel']/p/span[@data-purpose='error']")
    private lateinit var inLineError: WebElementFacade

    @FindBy(xpath = "//*[@data-purpose='error-heading']")
    private lateinit var errorSummaryHeading: WebElementFacade

    @FindBy(xpath = "//*[@data-purpose='error']")
    private lateinit var errorSummaryBody: WebElementFacade

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
        val dateTimeFormat = SimpleDateFormat("h:mm a EEEE dd MMMM yyyy")
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
        return inLineError.text
    }

    fun getErrorSummaryHeading(): String {
        return errorSummaryHeading.text
    }

    fun getErrorSummaryBody(): String {
        return errorSummaryBody.text
    }

    private fun convertToSlotObject(parentContainer: WebElementFacade, parentToSlotDivRelativePath: String = "", isMyAppointmentSlot: Boolean = false): Slot {
        val slot = Slot()
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) xPathRoot else "$parentToSlotDivRelativePath/"
        slot.time = findByXpath(parentContainer, relativePath + appointmentTimeXpath).text
        slot.session = findByXpath(parentContainer, relativePath + appointmentSessionNameXpath).text
        slot.date = findByXpath(parentContainer, relativePath + appointmentDateXpath).text

        val locationElement = findByXpath(parentContainer, relativePath + appointmentLocationXpath)
        slot.location = getSlotChildElementDisplayingText(locationElement)

        retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath + appointmentClinicianXPath, isMyAppointmentSlot)
        return slot
    }

    private fun retrieveDateTimeFromSlotElement(slotElement: WebElementFacade): String {
        val time = findByXpath(slotElement, appointmentTimeXpath).text
        val date = findByXpath(slotElement, appointmentDateXpath).text
        return "$time $date"
    }

    private fun retrieveAppointmentSlotDivs(containerDivXpath: String): List<WebElementFacade> {
        val slotDivs = findAllByXpath("$containerDivXpath/div")
        return if (slotDivs.size > 1) {
            slotDivs.subList(1, slotDivs.size)
        } else slotDivs
    }

    private fun retrieveAppointmentSlotDivAtPosition(containerDivXpath: String, index: Int): WebElementFacade {
        return findByXpath("$containerDivXpath/div[$index]")
    }

    private fun retrieveClinicianAndAddToSlot(slot: Slot, parentContainer: WebElementFacade, relativePath: String, isMyAppointmentSlot: Boolean) {
        if (isMyAppointmentSlot) {
            val clinicianElements = findAllByXpath(parentContainer, "$relativePath/*/span[starts-with(@aria-label, 'clinician')]")
            clinicianElements.forEach { clinicianElement ->
                val clinicianDisplayName = getSlotChildElementDisplayingText(clinicianElement)
                slot.clinician.add(clinicianDisplayName)
            }
        } else {
            val clinicians: List<WebElement> = findAllByXpath(parentContainer, relativePath + "ul/li")
            clinicians.forEach { clinician ->
                val clinicianDisplayName = getSlotChildElementDisplayingText(clinician)
                slot.clinician.add(clinicianDisplayName)
            }
        }
    }

    private fun getSlotChildElementDisplayingText(childElement: WebElement): String {
        val childElementText = childElement.text
        return try {
            val svg = childElement.findElement(By.tagName("svg"))
            childElementText.replace(svg.text, "").trim()
        } catch (e: Exception) {
            childElementText.trim()
        }
    }
}
