package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/appointments")
class MyAppointmentsPage : AppointmentSharedElementsPage() {

    @FindBy(id = "btn_floating")
    lateinit var bookButton: WebElementFacade

    @FindBy(xpath = "//*[@id='success-dialog']/div/p")
    private lateinit var successMessage: WebElementFacade

    @FindBy(xpath = "//h3/..")
    private lateinit var actualNoUpcomingText: WebElementFacade

    @FindBy(xpath = "//a[contains(text(),'Cancel appointment')]")
    private lateinit var cancelAppointmentLink: WebElementFacade

    private val upcomingAppointmentParentXpath = "//div[@data-purpose='appointments']"

    fun getSuccessMessage(): String = successMessage.text

    fun getNoUpcomingText(): String = actualNoUpcomingText.text

    fun getDateTimestampsOfSlots(): List<Long> {
        return getDateTimestampsOfSlots(upcomingAppointmentParentXpath)
    }

    fun getAllSlots(): ArrayList<Slot> {
        return getAllSlots(upcomingAppointmentParentXpath)
    }

    fun getSlotAtIndex(index: Int): Slot {
        return getSlotAtIndex(upcomingAppointmentParentXpath, index)
    }

    fun clickFirstCancelAppointmentLink() {
        cancelAppointmentLink.click()
    }
}
