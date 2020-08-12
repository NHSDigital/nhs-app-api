package pages.patientPracticeMessaging

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed

class PatientPracticeMessagingDeletePage: HybridPageObject() {

    fun assertDisplayed() {
        getParagraph("deleteContentPara1").assertIsDisplayed("Expected paragraph deleteContentPara1")
        getParagraph("deleteContentPara2").assertIsDisplayed("Expected paragraph deleteContentPara2")
        deleteButton().assertIsDisplayed("Expected delete button")
    }

    private fun pageElement(xPath: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = xPath,
                androidLocator = null,
                page = this
        )
    }

    private fun getParagraph(paragraphId: String) : HybridPageElement {
        return pageElement("//p[@id='$paragraphId']")
    }

    private fun deleteButton() : HybridPageElement {
        return pageElement("//button[@id='deleteButton']")
    }

    fun clickDeleteConversation() {
        deleteButton().click()
    }
}
