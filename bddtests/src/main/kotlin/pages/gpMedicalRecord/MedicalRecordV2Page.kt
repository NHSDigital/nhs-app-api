package pages.gpMedicalRecord

import models.Patient
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.LinkElement
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.text

class MedicalRecordV2Page : HybridPageObject() {

    val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"Your GP health record\")]",
            androidLocator = null,
            page = this,
            helpfulName = "GP Health Record Title")

    val clinicalAbbreviationsLink =
            HybridPageElement(
                    webDesktopLocator = "//a/span[contains(text(),'Help with abbreviations')]",
                    androidLocator = null,
                    page = this)

    private val noSummaryCareAccessMessage =
            HybridPageElement(
                    webDesktopLocator = "//div[@id='errorMsg']",
                    androidLocator = null,
                    page = this)

    fun getBody(message: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//p[contains(text(), \"$message\")]",
                androidLocator = null,
                page = this)
    }

    fun assertMedicalRecordSectionLinkExists(linkText: String) {
        medicalRecordSectionLink(linkText).assertSingleElementPresent()
    }

    fun clickMedicalRecordSectionLink(linkText: String) {
        medicalRecordSectionLink(linkText).click()
    }

    private fun medicalRecordSectionLink(linkText: String): LinkElement {
        val linkContent = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='medical-record-menu']")
                .addLink(linkText)
        val linkElement by lazy { LinksElement(this, linkContent) }

        return linkElement.link(linkText)
    }

    fun assertDemographicsContent(patient: Patient) {
        val fullContent =
                ExpectedPageStructure()
                        .h2(patient.formattedFullName())
                        .paragraph("Date of birth")
                        .paragraph(patient.age.formattedDateOfBirth())
                        .paragraph("NHS number")
                        .paragraph(patient.formattedNHSNumber())
                        .paragraph("Address")
                        .paragraph(patient.contactDetails.address.full())
        ExpectedPageStructureAssertor().assert(this, fullContent.build())
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.text
    }
}
