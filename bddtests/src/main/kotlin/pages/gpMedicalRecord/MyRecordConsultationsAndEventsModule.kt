package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordConsultationsAndEventsModule : HybridPageObject() {

    private val consultationsLinkText = "Consultations and events"
    private val consultations by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(consultationsLinkText)

    val link = consultations.link(consultationsLinkText)

}