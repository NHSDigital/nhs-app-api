package pages.prescription

import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.asciiText
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions")
open class PrescriptionsPage : HybridPageObject() {

    val timeoutPageHeader = "Prescription data error"
    val timeoutHeader = "There's been a problem getting your prescription information"
    val timeoutMessage = "Try again now. If the problem continues and you need this information now," +
            " contact your GP surgery directly. For urgent medical advice, call 111."
    val timeoutRetryButtonText = "Try again"

    val serverErrorPageHeader = "Prescription data error"
    val serverErrorHeader = "There\'s been a problem getting your prescription information"
    val serverErrorMessage = "Try again later. If the problem continues and you need this information now," +
            " contact your GP surgery directly. For urgent medical advice, call 111."

    private val orderARepeatPrescriptionButtonLocator = "//button[contains(text(), " +
                                                        "'Order new repeat prescription')]"
    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("My repeat prescriptions")
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

        val orderDateXpath = ".//*[@data-label='order-date']"
        val courseNameXpath = ".//*[@data-label='course-name']"
        val dosageXpath = ".//*[@data-label='detail']"
        val statusXpath = ".//*[@data-label='status']"

        prescriptions.forEach { el ->

            val p: HistoricPrescription

            if (allFieldsProvided) {
                p = HistoricPrescription(
                        name = el.findElement(By.xpath(courseNameXpath)).asciiText,
                        dosage = el.findElement(By.xpath(dosageXpath)).asciiText)

                p.orderDate = el.findElement(By.xpath(orderDateXpath)).asciiText
                p.status = el.findElement(By.xpath(statusXpath)).asciiText
            } else {
                p = HistoricPrescription(
                        name = el.findElement(By.xpath(courseNameXpath)).asciiText,
                        dosage = el.findElement(By.xpath(dosageXpath)).asciiText)
            }

            historicPrescriptions.add(p)
        }

        return historicPrescriptions
    }

    fun clickOrderARepeatPrescriptionButton ()
    {
        val orderARepeatPrescriptionButton = findByXpath(orderARepeatPrescriptionButtonLocator)
        orderARepeatPrescriptionButton.click()
    }
}
