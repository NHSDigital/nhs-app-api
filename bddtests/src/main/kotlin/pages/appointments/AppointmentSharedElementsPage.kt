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
    private val dataLabelXpath = "[@data-label='%s']"
    private val appointmentDateXpath = String.format(dataLabelXpath, "date")
    protected val appointmentTimeXpath = String.format(dataLabelXpath, "start time")
    protected val appointmentSessionNameXpath = String.format(dataLabelXpath, "session name")
    private val appointmentSlotTypeXpath = String.format(dataLabelXpath, "slot type")
    private val appointmentLocationXpath = String.format(dataLabelXpath, "location")
    private val appointmentCliniciansXPath = "[starts-with(@data-label, 'clinician')]"

    private val selectedAppointmentParentXpath = "//div[@aria-label='selected appointment']"

    val reasonError = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='reason-error']",
            androidLocator = null,
            page = this,
            helpfulName = "Reason Error"
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

    open fun getAppointmentSlot(areCliniciansExpected: Boolean = false): Slot {
        val slotsArray = getAllSlots(selectedAppointmentParentXpath, areCliniciansExpected)
        return slotsArray[0]
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

