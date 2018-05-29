package pages

import models.Slot
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.WebElement

@DefaultUrl("http://localhost:3000/appointments")
open class AppointmentsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(className = "info")
    lateinit var infoMessage: WebElementFacade

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
                slot.clinictian.add(clinicianDisplayName)
            }

            slotList.add(slot)
        }

        return slotList
    }

    fun getInformationMessage(): WebElementFacade {
        return infoMessage
    }
}
