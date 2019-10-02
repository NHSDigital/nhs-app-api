package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class MyRecordTestResultsModule(page : HybridPageObject) : LinksElement(page, content) {
    val testResults by lazy { link(testResultsLink) }

    companion object {
        private const val testResultsLink = "Test results"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='medical-record-menu']")
                .addLink(testResultsLink)
    }
}