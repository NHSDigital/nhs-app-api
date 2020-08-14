package pages.prescription

import com.google.gson.GsonBuilder
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.asciiText
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/view-orders")
open class ViewOrdersPrescriptionsPage : HybridPageObject() {

    val timeoutPageHeader = "Prescription data error"
    val timeoutHeader = "There's been a problem getting your prescription information"
    val timeoutMessage = "Try again now. If the problem continues and you need this information now," +
            " contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111."
    val timeoutRetryButtonText = "Try again"

    val serverErrorPageHeader = "Prescription data error"
    val serverErrorHeader = "There\'s been a problem getting your prescription information"
    val serverErrorMessage = "Try again later. If the problem continues and you need this information now," +
            " contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111."

    val orderedByLabel = HybridPageElement(
            webDesktopLocator = "//p[@id='orderedByValue']",
            helpfulName = "ordered by label",
            page = this
    )

    val changeNominatedPharmacyLink = HybridPageElement(
            webDesktopLocator = "//a[@id='change-link']",
            helpfulName = "ordered by label",
            page = this
    )

    val noSetNominatedPharmacyHelpText =  HybridPageElement(
            webDesktopLocator = "//p[@id='no-nominated-pharmacy']",
            helpfulName = "no nominated pharmacy help text",
            page = this
    )

    fun getNominatedPharmacyName(): String {
        val nominatedPharmacyName = findByXpath("//p[@id='pharmacy-name']")
        return nominatedPharmacyName.text
    }

    private lateinit var webHeader: WebHeader

    fun isLoaded() {
        webHeader.getPageTitle().withText("Your orders")
    }

    fun isNoPrescriptionsMessageVisible(): Boolean {
        // note: needs double quotes in "contains" expression because message has apostrophe
        val message = "You don't currently have any repeat prescriptions ordered"
        return findByXpath("//h2[contains(., \"$message\")]").isVisible
    }

    fun iClickTheChangeNominatedPharmacyLink() {
        changeNominatedPharmacyLink.click()
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
        val statusPath = ".//*[@data-label='status-text']"

        prescriptions.forEach { el ->

            val p: HistoricPrescription

            if (allFieldsProvided) {
                p = HistoricPrescription(
                        name = el.findElement<WebElement>(By.xpath(courseNameXpath)).asciiText,
                        dosage = el.findElement<WebElement>(By.xpath(dosageXpath)).asciiText)

                p.orderDate = el.findElement<WebElement>(By.xpath(orderDateXpath)).asciiText
                p.status = el.findElement<WebElement>(By.xpath(statusPath)).asciiText
            } else {
                p = HistoricPrescription(
                        name = el.findElement<WebElement>(By.xpath(courseNameXpath)).asciiText,
                        dosage = el.findElement<WebElement>(By.xpath(dosageXpath)).asciiText)
            }

            historicPrescriptions.add(p)
        }

        return historicPrescriptions
    }

    fun assertPrescriptionsMatch(list: List<HistoricPrescription>,
                                 expectedPrescriptions: Int,
                                 providerHasAllPrescriptionFields: Boolean = true) {

        val gson = GsonBuilder().setPrettyPrinting().create()
        val p = getAllPrescriptions(providerHasAllPrescriptionFields)

        val actualJson = gson.toJson(p).toString()
        val expectedJson = gson.toJson(list).toString()

        Assert.assertEquals(expectedJson, actualJson)
        Assert.assertEquals(expectedPrescriptions, p.count())
    }
}
