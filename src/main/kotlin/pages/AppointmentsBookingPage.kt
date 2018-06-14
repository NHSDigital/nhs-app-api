package pages

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.WebElement

@DefaultUrl("http://localhost:3000/appointments/booking")
open class AppointmentsBookingPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(className = "info")
    lateinit var infoMessage: WebElementFacade

    @FindBy(xpath = "//button[contains(text(),'Book this appointment')]")
    lateinit var bookAppointmentButton: WebElementFacade

    fun getAllSlots(): ArrayList<Slot> {
        val slotList = ArrayList<Slot>()

        val appointmentSlots = findAllByXpath("//ul[@data-purpose='slots']/li")
        appointmentSlots.forEach { el ->
            var slot = Slot()

            slot.time = el.findElement(By.xpath("//*[@class='start-time']")).text
            slot.session = el.findElement(By.xpath("//*[@class='session']")).text
            slot.date = el.findElement(By.xpath("//*[@class='date']")).text

            val locationElement = el.findElement(By.xpath("//*[@class='location']"))
            slot.location = locationElement.text.replace(locationElement.findElement(By.xpath("//title")).text, "")

            val clinicians: List<WebElement> = el.findElement(By.xpath("//*[@class='clinicians']")).findElements(By.xpath("/li"))
            clinicians.forEach { clinician ->
                val clinicianDisplayName = clinician.text.replace(clinician.findElement(By.xpath("//title")).text, "")
                slot.clinician.add(clinicianDisplayName)
            }

            slotList.add(slot)
        }

        return slotList
    }

    fun countSlots(): Int{
        return findAllByXpath("//ul[@data-purpose='slots']/li").size
    }

    fun getServerErrorElement(): WebElementFacade {
        return findBy<WebElementFacade>("#serverError").waitUntilVisible<WebElementFacade>();
    }

    fun selectFirstSlot()
    {
        findAllByXpath("//ul[@data-purpose='slots']/li").first().waitUntilClickable<WebElementFacade>().click()
    }

    fun clickOnBookAppointmentButton()
    {
        bookAppointmentButton.waitUntilClickable<WebElementFacade>().click()
    }

    fun getServerErrorMessage(): String {
        return getServerErrorElement().findElement(By.className("msg")).text
    }

    fun getTryAgainButton(): WebElement
    {
        return getServerErrorElement().findElement(By.className("button"))
    }

    fun hasTryAgainButton(): Boolean
    {
        return getServerErrorElement().containsElements(By.className("button"))
    }
}
