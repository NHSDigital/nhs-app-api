package pages.patientPracticeMessaging

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import pages.text
import utils.DownloadHelpers

class PatientPracticeDownloadAttachmentPage: HybridPageObject() {

    fun assertInformationParagraph() {
        val informationParagraph = getParagraph("downloadInformation")
        informationParagraph.isDisplayed
        Assert.assertEquals("When you download this file, you become responsible for keeping it confidential.",
                            informationParagraph.text)

    }

    fun assertDownloadButtonDisplayed() {
        downloadButton().isDisplayed
        Assert.assertEquals(downloadButton().text, "Download file")
    }

    fun assertInvalidMessage() {
       pageElement("//h1[normalize-space(text())='This file is not available in the NHS App']")
                .isDisplayed
    }

    fun hasAttachmentDownloaded(expectedFileName: String): Boolean {
        return DownloadHelpers().downloadFile(expectedFileName)
    }

    private fun pageElement(xPath: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = xPath,
                androidLocator = null,
                page = this)
    }

    private fun getParagraph(paragraphId: String) : HybridPageElement {
        return pageElement("//p[@id='$paragraphId']")
    }

    private fun downloadButton() : HybridPageElement {
        return pageElement("//button[@id='downloadButton']")
    }

    fun downloadButtonClicked() {
        downloadButton().click()
    }
}