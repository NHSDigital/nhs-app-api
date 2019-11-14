package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordMedicalHistoryModule : HybridPageObject() {

    private val medicalHistoryLinkText = "Medical history"
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(medicalHistoryLinkText)
    private val medicalHistory by lazy {LinksElement(this, content)}

    val link = medicalHistory.link(medicalHistoryLinkText)

}