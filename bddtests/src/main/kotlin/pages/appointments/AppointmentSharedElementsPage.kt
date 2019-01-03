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
    private val appointmentTimeXpath = "[@data-label='start time']"
    private val appointmentSessionNameXpath = "[@data-label='session name']"
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
        slot.time = findByXpath(parentContainer, relativePath + appointmentTimeXpath).text
        slot.session = findByXpath(parentContainer, relativePath + appointmentSessionNameXpath).text
        slot.date = findByXpath(parentContainer, relativePath + appointmentDateXpath).text
        slot.location = findByXpath(parentContainer, relativePath + appointmentLocationXpath).text

        if (areCliniciansExpected)
            retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath)
        return slot
    }

    private fun retrieveDateTimeFromSlotElement(slotElement: WebElementFacade): String {
        val time = findByXpath(slotElement, xPathRoot + appointmentTimeXpath).text
        val date = findByXpath(slotElement, xPathRoot + appointmentDateXpath).text
        return "$time $date"
    }

    private fun retrieveAppointmentSlotDivs(containerDivXpath: String): List<WebElementFacade> {
        return findAllByXpath(containerDivXpath)
    }

    private fun retrieveClinicianAndAddToSlot(slot: Slot, parentContainer: WebElementFacade, relativePath: String) {
        val cliniciansPresent = driver.findElements(
                By.xpath("$xPathRoot//span$appointmentCliniciansXPath")).size > 0
        if (!cliniciansPresent) return

        val clinicians = findAllByXpath(parentContainer, "$relativePath//span$appointmentCliniciansXPath")

        clinicians.forEach { clinician ->
            slot.clinicians = slot.clinicians.plus(clinician.text)
        }
    }
}
