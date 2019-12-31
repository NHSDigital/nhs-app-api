package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.HybridPageElement
import pages.assertIsVisible
import pages.sharedElements.LinksContent

class MedicalRecordV2Page : HybridPageObject() {

    val pageTitle = HybridPageElement(
                    webDesktopLocator = "//h1[contains(text(),\"Your GP medical record\")]",
                    androidLocator = null,
                    page = this,
                    helpfulName = "GP Medical Record Title")

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    webDesktopLocator = "//a[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    val patientName =
            HybridPageElement(
                    webDesktopLocator = "//h2[@data-sid='patient-name']",
                    page = this,
                    helpfulName = "Patient name"
            )

    val dateOfBirth =
            HybridPageElement(
                    webDesktopLocator = "//p[@data-sid='user-date-of-birth']",
                    page = this,
                    helpfulName = "Patient date of birth"
            )

    val nhsNumber =
            HybridPageElement(
                    webDesktopLocator = "//p[@data-sid='user-nhs-number']",
                    page = this,
                    helpfulName = "Patient nhs number"
            )

    val address =
            HybridPageElement(
                    webDesktopLocator = "//p[@data-sid='user-address']",
                    page = this,
                    helpfulName = "Patient address"
            )

    fun getBody(message: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//p[contains(text(), \"$message\")]",
                androidLocator = null,
                page = this)
    }

    fun assertMedicalRecordSectionLinkExists(linkText: String) {
        medicalRecordSectionLink(linkText).assertIsVisible()
    }

    fun clickMedicalRecordSectionLink(linkText: String) {
        medicalRecordSectionLink(linkText).click()
    }

    private fun medicalRecordSectionLink(linkText: String): HybridPageElement {
        val linkContent = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='medical-record-menu']")
                .addLink(linkText)
        val linkElement by lazy {LinksElement(this, linkContent)}

        return linkElement.link(linkText);
    }
}
