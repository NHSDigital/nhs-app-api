package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import java.text.SimpleDateFormat
import java.util.*

@DefaultUrl("http://localhost:3000/appointments")
class MyAppointmentsPage : AppointmentSharedElementsPage() {

    @FindBy(id = "btn_floating")
    lateinit var bookButton: WebElementFacade

    @FindBy(id = "success-dialog")
    lateinit var successMessage: WebElementFacade

    @FindBy(xpath = "//h3/..")
    lateinit var actualNoUpcomingText: WebElementFacade

    val appointmentSlotParentXpath = "//*[@class='panel-title']/.."

    fun getSuccessMessage(): String = successMessage.text

    fun getNoUpcomingText(): String = actualNoUpcomingText.text

    fun getDateTimestampsOfSlots(): List<Long> {
        val slotTimestamps = arrayListOf<Long>()
        val dateTimeFormat = SimpleDateFormat("h:mm a EEEE dd MMMM yyyy")
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentSlotParentXpath)

        appointmentSlotDivs.forEach { slotDiv ->
            val timestamp = retrieveDateTimeFromSlotElement(slotDiv)
            slotTimestamps.add(dateTimeFormat.parse(timestamp).time)
        }
        return slotTimestamps
    }

    fun getAllSlots(): ArrayList<Slot> {
        val slotList = arrayListOf<Slot>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs(appointmentSlotParentXpath)

        appointmentSlotDivs.forEach { slotDiv ->
            val slot = convertToSlotObject(slotDiv, isMyAppointmentSlot = true)
            slotList.add(slot)
        }
        return slotList
    }

    private fun retrieveDateTimeFromSlotElement(slotElement: WebElementFacade): String {
        val time = findByXpath(slotElement, "//h4").text
        val date = findByXpath(slotElement, "//h5").text
        return "$time $date"
    }

    private fun retrieveAppointmentSlotDivs(containerDivXpath: String): List<WebElementFacade> {
        val slotDivs = findAllByXpath("$containerDivXpath/div")
        return if (slotDivs.size > 1) {
            slotDivs.subList(1, slotDivs.size)
        } else slotDivs
    }
}
