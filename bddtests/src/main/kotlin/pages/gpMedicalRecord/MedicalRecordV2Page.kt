package pages.gpMedicalRecord

import models.Patient
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.sharedElements.LinkElement
import pages.sharedElements.LinksContent
import pages.sharedElements.LinksElement
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.text

class MedicalRecordV2Page : HybridPageObject() {

    private val askForDcrAccessSelector = "//div[@data-purpose='inset-text']/p"
    private val findOutMoreAboutAccessLinkText = "Find out more about requesting access."
    private val askForDcrAccessContent = "Ask your GP surgery for access to your detailed coded record. You'll only " +
            "need to do this once to view information like test results, immunisations and more. " +
            findOutMoreAboutAccessLinkText

    val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"Your GP health record\")]",
            androidLocator = null,
            page = this,
            helpfulName = "GP Health Record Title")

    val askForDcrAccessInsetText = HybridPageElement(
            webDesktopLocator = askForDcrAccessSelector,
            page = this,
            helpfulName = "Ask for DCR access info"
    )

    val askForDcrAccessInsetTextLink = HybridPageElement(
            webDesktopLocator = "$askForDcrAccessSelector/a",
            page = this,
            helpfulName = "Ask for DCR access info - Link"
    )

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
                        .paragraph("Date of birth:")
                        .paragraph(patient.age.formattedDateOfBirth())
                        .paragraph("NHS number:")
                        .paragraph(patient.formattedNHSNumber())
                        .paragraph("Address:")
                        .paragraph(patient.contactDetails.address.full())
        ExpectedPageStructureAssertor().assert(this, fullContent.build())
    }

    fun assertInfoAskingForDcrAccessVisible() {
        askForDcrAccessInsetText.assertIsVisible()
        Assert.assertEquals(askForDcrAccessContent, askForDcrAccessInsetText.textValue)

        askForDcrAccessInsetTextLink.assertIsVisible()
        Assert.assertEquals(findOutMoreAboutAccessLinkText, askForDcrAccessInsetTextLink.textValue)
    }

    fun getSummaryCareNoAccessMessage(): String {
        return noSummaryCareAccessMessage.text
    }
}
