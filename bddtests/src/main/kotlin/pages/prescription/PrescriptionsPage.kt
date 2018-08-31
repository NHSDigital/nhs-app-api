package pages.prescription

import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.Header
import pages.HybridPageObject.Companion.PageType

@DefaultUrl("http://localhost:3000/prescriptions")
open class PrescriptionsPage : HybridPageObject(PageType.WEBVIEW_APP) {

    lateinit var headerBar: Header

    val timeoutPageTitle = "Prescription data error"
    val timeoutPageHeader = "Error retrieving data"
    val timeoutHeader = "Sorry, there\'s been a problem getting your prescription information"
    val timeoutSubHeader = "Please try again"
    val timeoutMessage = "If the problem persists and you need this " +
                         "information now, please contact your GP surgery directly."
    val timeoutRetryButtonText = "Try again"

    val serverErrorPageTitle = "Prescription data error"
    val serverErrorPageHeader = "Error retrieving data"
    val serverErrorHeader = "Sorry, there\'s been a problem getting your prescription information"
    val serverErrorSubHeader = "Please try again later. If the problem " +
                               "persists and you need this information now, " +
                               "please contact your GP surgery directly."
    val serverErrorMessage = ""
    val serverErrorretryButtonText = ""

    private val orderARepeatPrescriptionButtonLocator = "//button[contains(text(), " +
                                                        "'Order new repeat prescription')]"

    fun isLoaded(): Boolean {
        return headerBar.isVisible("My repeat prescriptions")
    }

    fun isNoPrescriptionsMessageVisible(): Boolean {
        // note: needs double quotes in "contains" expression because message has apostrophe
        val message = "You don't currently have any repeat prescriptions ordered"
        return findByXpath("//h2[contains(., \"$message\")]").isVisible
    }

    fun isOrderSuccessfullTextVisible(): Boolean {
        val successText = "Your prescription has been ordered."
        return findByXpath("//div[@id='success-dialog']//p[contains(.,'$successText')]").isVisible
    }

    fun getAllPrescriptions(allFieldsProvided: Boolean): List<HistoricPrescription> {
        val historicPrescriptions = ArrayList<HistoricPrescription>()

        val prescriptions = findAllByXpath("//div[@data-label='historic-prescription']")

        var orderDateXpath = ".//*[@data-label='order-date']"
        var courseNameXpath = ".//*[@data-label='course-name']"
        var dosageXpath = ".//*[@data-label='detail']"
        var statusXpath = ".//*[@data-label='status']"

        prescriptions.forEach( { el ->

            var p: HistoricPrescription

            if (allFieldsProvided) {
                p = HistoricPrescription(
                        name = el.findElement(By.xpath(courseNameXpath)).text,
                        dosage = el.findElement(By.xpath(dosageXpath)).text)

                p.orderDate = el.findElement(By.xpath(orderDateXpath)).text
                p.status = el.findElement(By.xpath(statusXpath)).text
            } else {
                p = HistoricPrescription(
                        name = el.findElement(By.xpath(courseNameXpath)).text,
                        dosage = el.findElement(By.xpath(dosageXpath)).text)
            }

            historicPrescriptions.add(p)
        })

        return historicPrescriptions
    }

    fun clickOrderARepeatPrescriptionButton ()
    {
        val orderARepeatPrescriptionButton = findByXpath(orderARepeatPrescriptionButtonLocator)
        orderARepeatPrescriptionButton.click()
    }
}
