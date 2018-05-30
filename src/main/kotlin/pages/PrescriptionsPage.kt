package pages

import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.annotations.findby.By
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.Header

@DefaultUrl("http://localhost:3000/prescriptions")
open class PrescriptionsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    lateinit var headerBar: Header

    private val orderARepeatPrescriptionButtonLocator = "//button[contains(text(), 'Order a repeat prescription')]"

    fun isLoaded(): Boolean {
        return headerBar.isVisible("My repeat prescriptions")
    }

    fun isNoPrescriptionsMessageVisible(): Boolean {
        val message = "Looks like you have no repeat prescriptions ordered here."
        return findByXpath("//div[@class='info']//b[contains(.,'$message')]").isVisible
    }

    fun getAllPrescriptions(): ArrayList<HistoricPrescription> {
        val historicPrescriptions = ArrayList<HistoricPrescription>()

        val prescriptions = findAllByXpath("//ul[@data-purpose='prescriptions']/li")

        prescriptions.forEach( { el ->
            var p = HistoricPrescription(
                    orderDate = el.findElement(By.xpath(".//*[@aria-label='order-date']")).text,
                    name = el.findElement(By.xpath(".//*[@aria-label='course-name']")).text,
                    dosage = el.findElement(By.xpath(".//*[@aria-label='dosage']")).text
            )
            historicPrescriptions.add(p)
        })

        return historicPrescriptions
    }
    fun doesOrderARepeatPrescriptionButtonExist () : Boolean {

        val orderARepeatPrescriptionButton = findByXpath(orderARepeatPrescriptionButtonLocator)
        return orderARepeatPrescriptionButton.isPresent
    }

    fun clickOrderARepeatPrescriptionButton ()
    {
        val orderARepeatPrescriptionButton = findByXpath(orderARepeatPrescriptionButtonLocator)
        orderARepeatPrescriptionButton.click()
    }


}
