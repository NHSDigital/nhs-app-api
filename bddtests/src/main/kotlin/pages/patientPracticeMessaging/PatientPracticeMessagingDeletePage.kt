package pages.patientPracticeMessaging

import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed

class PatientPracticeMessagingDeletePage: HybridPageObject() {

    fun assertDisplayed() {
        getParagraph("deleteContentPara1").isDisplayed
        getParagraph("deleteContentPara2").isDisplayed
        deleteButton().isDisplayed
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