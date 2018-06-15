package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.JavascriptExecutor
import java.text.SimpleDateFormat
import java.util.*

@DefaultUrl("http://localhost:3000/appointments")
class MyAppointmentsPage : AppointmentSharedElementsPage() {
    val pageHeader by lazy { "My appointments" }
    val noUpcomingHeader by lazy { "You don't currently have any appointments booked" }
    val bookingSuccessMessage by lazy { "Appointment Booked" }
    val bookAnButtonText by lazy { "Book an appointment" }

    @FindBy(id = "btn_floating")
    lateinit var bookButton: WebElementFacade

    fun getSuccessMessage(): String = find<WebElementFacade>(By.id("success-dialog")).text

    fun getNoUpcomingHeaderText(): String = findByXpath("//*[@id='mainDiv']/main[2]/div[1]/h3").text

    fun getDateTimestampsOfSlots(): List<Long> {
        val slotTimestamps = arrayListOf<Long>()
        val dateTimeFormat = SimpleDateFormat("h:mm a EEEE dd MMMM yyyy")
        val appointmentSlotDivs = retrieveAppointmentSlotDivs("//*[@id='mainDiv']/main[2]/div[1]")
        appointmentSlotDivs.forEach { slotDiv ->
            val timestamp = retrieveDateTimeFromSlotElement(slotDiv)
            slotTimestamps.add(dateTimeFormat.parse(timestamp).time)
        }
        return slotTimestamps
    }

    fun getAllSlots(): ArrayList<Slot> {
        val slotList = arrayListOf<Slot>()
        val appointmentSlotDivs = retrieveAppointmentSlotDivs("//*[@id='mainDiv']/main[2]/div[1]")

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
