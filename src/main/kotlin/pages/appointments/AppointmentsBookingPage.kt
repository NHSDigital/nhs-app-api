package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.WebElement

@DefaultUrl("http://localhost:3000/appointments/booking")
class AppointmentsBookingPage : AppointmentSharedElementsPage() {
    val pageHeader by lazy { "Book an appointment" }
    val bookThisButtonText by lazy { "Book this appointment" }

    @FindBy(xpath = "//div[@id='mainDiv']/div[@class='content']")
    lateinit var serverError: WebElementFacade

    fun selectFirstSlot() {
        findAllByXpath("//ul[@data-purpose='slots']/li").first().waitUntilClickable<WebElementFacade>().click()
    }

    fun getServerErrorMessage(): String? {
        val message: String?
        if (serverError.isPresent) {
            message = serverError.text
        }
        else message = null

        return message
    }

    fun getTryAgainButton(): WebElement {
        return serverError.waitUntilVisible<WebElementFacade>().findElement(By.className("button"))
    }

    fun hasTryAgainButton(): Boolean {
        return serverError.containsElements(By.className("button"))
    }

    fun countSlots(): Int {
        return findAllByXpath("//ul[@data-purpose='slots']/li").size
    }

    fun getAllSlots(): ArrayList<Slot> {
        val slots = arrayListOf<Slot>()
        val slotElementList = findAllByXpath("//ul[@data-purpose='slots']/li")
        slotElementList.forEach { slotLiElement ->
            val slot = convertToSlotObject(slotLiElement, "./div")
            slots.add(slot)
        }
        return slots
    }
}
