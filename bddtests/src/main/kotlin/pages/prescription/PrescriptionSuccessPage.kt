package pages.prescription

import org.junit.Assert
import pages.HybridPageObject
import pages.HybridPageElement
import pages.text

class PrescriptionSuccessPage : HybridPageObject() {

    private val prescriptionSuccessMessage = "Your prescription has been ordered"

    private val backLinkText = "Go to your prescription orders"

    private val successMessage = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(),\"$prescriptionSuccessMessage\")]",
        page = this
    )

    private val backLinkTextElement = HybridPageElement(
        webDesktopLocator = "//a[contains(text(),\"$backLinkText\")]",
        page = this
    )

    fun checkPrescriptionSuccessMessage() {
        Assert.assertEquals(prescriptionSuccessMessage, successMessage.text)
    }

    fun clickBackLink() {
        backLinkTextElement.click()
    }
}

