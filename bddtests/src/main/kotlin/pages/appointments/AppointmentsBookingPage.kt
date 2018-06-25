package pages.appointments

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import java.util.ArrayList

@DefaultUrl("http://localhost:3000/appointments/booking")
class AppointmentsBookingPage : AppointmentSharedElementsPage() {

    @FindBy(xpath = "//div[@id='mainDiv']/div[@class='content']")
    lateinit var serverError: WebElementFacade

    private val appointmentSlotParentXpath = "//ul[@data-purpose='slots']/li"

    fun selectFirstSlot() {
        findAllByXpath(appointmentSlotParentXpath).first().waitUntilClickable<WebElementFacade>().click()
    }

    fun getAllSlots(): ArrayList<Slot> {
        return getAllSlots(appointmentSlotParentXpath)
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
        return getAllSlots().size
    }

}
