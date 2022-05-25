package pages.patientPracticeMessaging

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed
import pages.text
import utils.DownloadHelpers

class PatientPracticeDownloadAttachmentPage: HybridPageObject() {

    fun assertInformationParagraph() {
        val informationParagraph = getParagraph("downloadInformation")
        Assert.assertEquals("When you download this file, you become responsible for keeping it confidential.",
                            informationParagraph.text)

    }

    fun assertDownloadButtonDisplayed() {
        Assert.assertEquals(downloadButton().text, "Download file")
    }

    fun assertInvalidMessage() {
       pageElement("//h1[normalize-space(text())='Cannot view file in the NHS App']")
                .assertIsDisplayed("Expected Invalid Message")
    }

    fun assertReasonsWhyFileCannotBeViewed() {
        pageElement("//p[normalize-space(text())='This file cannot be viewed because either:']")
            .assertIsDisplayed("Expected reason paragraph to be displayed")

        pageElement("//li[normalize-space(text())=\"it's too large\"]")
            .assertIsDisplayed("Expected first bullet point to be displayed")

        pageElement("//li[normalize-space(text())='the file type cannot be sent in the app']")
            .assertIsDisplayed("Expected second bullet point to be displayed")
    }

    fun hasAttachmentDownloaded(expectedFileName: String): Boolean {
        return DownloadHelpers().downloadFile(expectedFileName)
    }

    private fun pageElement(xPath: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = xPath,
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
