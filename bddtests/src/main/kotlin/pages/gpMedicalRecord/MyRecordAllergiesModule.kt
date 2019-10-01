package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

class MyRecordAllergiesModule(page : HybridPageObject) : LinksElement(page, content) {
    val allergies by lazy { link(allergiesLink) }

    companion object {
        private const val allergiesLink = "Allergies and adverse reactions"
        private var content = LinksContent(
                linkBlockTitle = "",
                containerXPath = "//div[@data-purpose='medical-record-menu']")
                .addLink(allergiesLink)
    }
}