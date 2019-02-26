package pages.appointments

import constants.DateTimeFormats.Companion.frontendDateFormat
import constants.DateTimeFormats.Companion.frontendTimeFormat
import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import java.text.SimpleDateFormat
import java.util.*

open class AppointmentSharedElementsPage : HybridPageObject() {
    private val xPathRoot = "//*"
    private val relativeToParentXPath = ".//*"
    private val appointmentDateXpath = "[@data-label='date']"
    protected val appointmentTimeXpath = "[@data-label='start time']"
    protected val appointmentSessionNameXpath = "[@data-label='session name']"
    private val appointmentSlotTypeXpath = "[@data-label='slot type']"
    private val appointmentLocationXpath = "[@data-label='location']"
    private val appointmentCliniciansXPath = "[contains(@data-label, 'clinician')]"

    val reasonError = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='reason-error']",
            androidLocator = null,
            page = this,
            helpfulName = "Reason Error"
    )

    val selectedAppointmentDate = HybridPageElement(
            webDesktopLocator = xPathRoot + appointmentDateXpath,
            androidLocator = null,
            page = this
    )

    val selectedAppointmentTime = HybridPageElement(
            webDesktopLocator = xPathRoot + appointmentTimeXpath,
            androidLocator = null,
            page = this
    )

    val selectedAppointmentSlotName = HybridPageElement(
            webDesktopLocator = xPathRoot + appointmentSlotTypeXpath,
            androidLocator = null,
            page = this
    )

    val selectedAppointmentSessionName = HybridPageElement(
            webDesktopLocator = xPathRoot + appointmentSessionNameXpath,
            androidLocator = null,
            page = this
    )

    val selectedAppointmentLocation = HybridPageElement(
            webDesktopLocator = xPathRoot + appointmentLocationXpath,
            androidLocator = null,
            page = this
    )

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

    fun getAllSlots(appointmentListParentXpath: String, areCliniciansExpected: Boolean): ArrayList<Slot> {
        val slotList = arrayListOf<Slot>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentListParentXpath)

        appointmentSlotDivs.forEach { slotDiv ->
            val slot = convertToSlotObject(slotDiv, areCliniciansExpected)
            slotList.add(slot)
        }
        return slotList
    }

    fun getSelectedAppointmentClinicianText(): Set<String> {
        return findAllByXpath(xPathRoot + appointmentCliniciansXPath).map { element -> element.text }.toSet()
    }

    private fun convertToSlotObject(parentContainer: WebElementFacade,
                                    areCliniciansExpected: Boolean = true,
                                    parentToSlotDivRelativePath: String = ""): Slot {
        val slot = Slot()
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) relativeToParentXPath
                           else "$parentToSlotDivRelativePath/"
        slot.time = parentContainer.findByXpath( relativePath + appointmentTimeXpath)!!.text
        slot.sessionName = parentContainer.findByXpath( relativePath + appointmentSessionNameXpath)?.text
        slot.slotType = parentContainer.findByXpath( relativePath + appointmentSlotTypeXpath)!!.text
        slot.date = parentContainer.findByXpath(relativePath + appointmentDateXpath)!!.text.trimEnd()
        slot.location = parentContainer.findByXpath(relativePath + appointmentLocationXpath)!!.text

        if (areCliniciansExpected)
            retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath)
        return slot
    }

    private fun retrieveDateTimeFromSlotElement(slotElement: WebElementFacade): String {
        val time = slotElement.findByXpath(xPathRoot + appointmentTimeXpath)?.text
        val date = slotElement.findByXpath(xPathRoot + appointmentDateXpath)?.text
        return "$time $date"
    }

    private fun retrieveAppointmentSlotDivs(containerDivXpath: String): List<WebElementFacade> {
        return findAllByXpath(containerDivXpath)
    }

    private fun retrieveClinicianAndAddToSlot(slot: Slot, parentContainer: WebElementFacade, relativePath: String) {
        val cliniciansPresent = driver.findElements(
                By.xpath("$xPathRoot//span$appointmentCliniciansXPath")).isNotEmpty()
        if (!cliniciansPresent) return
        val clinicians = parentContainer.thenFindAll(
                By.xpath("$relativePath//span$appointmentCliniciansXPath"))

        clinicians.forEach { clinician ->
            slot.clinicians = slot.clinicians.plus(clinician.text)
        }
    }
}

fun WebElementFacade.findByXpath(xpath: String): WebElementFacade? {
    val elements = thenFindAll(xpath)
    return elements.firstOrNull()
}

