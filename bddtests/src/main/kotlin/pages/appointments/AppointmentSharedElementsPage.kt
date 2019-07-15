package pages.appointments

import constants.DateTimeFormats.Companion.frontendDateFormat
import constants.DateTimeFormats.Companion.frontendTimeFormat
import models.Slot
import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isPresent
import pages.navigation.HeaderNative
import pages.withoutRetrying
import java.text.SimpleDateFormat
import java.util.*

abstract class AppointmentSharedElementsPage : HybridPageObject() {
    private val xPathRoot = "//*"
    private val relativeToParentXPath = ".//*"
    private val dataLabelXpath = "[@data-label='%s']"
    private val appointmentDateXpath = String.format(dataLabelXpath, "date")
    protected val appointmentTimeXpath = String.format(dataLabelXpath, "start time")
    protected val appointmentSessionNameXpath = String.format(dataLabelXpath, "session name")
    private val appointmentSlotTypeXpath = String.format(dataLabelXpath, "slot type")
    private val appointmentLocationXpath = String.format(dataLabelXpath, "location")
    private val appointmentCliniciansXPath = "[starts-with(@data-label, 'clinician')]"
    private val appointmentTelephoneXPath = String.format(dataLabelXpath, "phone number")

    private val selectedAppointmentParentXpath = "//div[@data-purpose='appointment-info']"
    lateinit var headerNative: HeaderNative

    val reasonError = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='reason-error']",
            androidLocator = null,
            page = this,
            helpfulName = "Reason Error"
    )

    val phoneConfirmation = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='phone-number']",
            page = this,
            helpfulName = "Telephone Confirmation Prompt"
    )

    abstract val titleText: String?

    fun assertPageFullyLoaded() {
        headerNative.getPageTitle(titleText!!)
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

    fun getAllSlots(appointmentListParentXpath: String, areCliniciansExpected: Boolean,
                    isTelephoneAppointment: Boolean): ArrayList<Slot> {
        val slotList = arrayListOf<Slot>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentListParentXpath)
        appointmentSlotDivs.forEach { slotDiv ->
            val slot = convertToSlotObject(slotDiv, areCliniciansExpected, isTelephoneAppointment)
            slotList.add(slot)
        }
        return slotList
    }

    open fun getAppointmentSlot(areCliniciansExpected: Boolean = false): Slot {
        val slotsArray = getAllSlots(selectedAppointmentParentXpath, areCliniciansExpected, false)
        return slotsArray[0]
    }

    private fun convertToSlotObject(parentContainer: WebElementFacade,
                                    areCliniciansExpected: Boolean = true,
                                    isTelephoneAppointment: Boolean = false,
                                    parentToSlotDivRelativePath: String = ""): Slot {
        val slot = Slot()
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) relativeToParentXPath
                           else "$parentToSlotDivRelativePath/"
        slot.time = parentContainer.findByXpath( relativePath + appointmentTimeXpath)!!.text
        slot.sessionName = parentContainer.findByXpath( relativePath + appointmentSessionNameXpath)?.text
        slot.slotType = parentContainer.findByXpath( relativePath + appointmentSlotTypeXpath)!!.text
        slot.date = parentContainer.findByXpath(relativePath + appointmentDateXpath)!!.text.trimEnd()
        slot.location = parentContainer.findByXpath(relativePath + appointmentLocationXpath)!!.text


        convertTelephoneNumber(slot, isTelephoneAppointment, parentContainer, relativePath)

        if (areCliniciansExpected)
            retrieveClinicianAndAddToSlot(slot, parentContainer, relativePath)
        return slot
    }

    private fun convertTelephoneNumber(slot: Slot,
                                       isTelephoneAppointment: Boolean,
                                       parentContainer: WebElementFacade,
                                       relativePath: String ) {
        slot.channel = "Unknown"

        if(phoneConfirmation.withoutRetrying().isPresent)
            slot.channel = "Telephone"
        else {
            if (isTelephoneAppointment)
                retrieveTelephoneNumber(slot, parentContainer, relativePath)

            retrieveTelephoneNumber(slot, parentContainer, relativePath)

            if (slot.telephoneNumber != "")
                slot.channel = "Telephone"

        }

        if(slot.telephoneNumber == "telephoneNumberToEnter" || slot.telephoneNumber == "on")
            slot.telephoneNumber = ""
    }

    private fun retrieveTelephoneNumber(slot: Slot, parentContainer: WebElementFacade, relativePath: String) {
        try {
            slot.telephoneNumber = parentContainer.findByXpath(relativePath +
                    appointmentTelephoneXPath)!!.text.split(" ").last()
        } catch( e: KotlinNullPointerException) { return }

    }

    fun getTelephoneSlot(appointmentListParentXpath: String): ArrayList<String> {
        val slotList = arrayListOf<String>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentListParentXpath)

        appointmentSlotDivs.forEach { slotDiv ->
            slotList.add(convertTelephoneField(slotDiv))
        }
        return slotList
    }

    private fun convertTelephoneField(parentContainer: WebElementFacade,
                                    parentToSlotDivRelativePath: String = ""): String {
        val relativePath = if (parentToSlotDivRelativePath.isEmpty()) relativeToParentXPath
        else "$parentToSlotDivRelativePath/"
        return parentContainer.findByXpath(relativePath + appointmentTelephoneXPath)!!.text
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

