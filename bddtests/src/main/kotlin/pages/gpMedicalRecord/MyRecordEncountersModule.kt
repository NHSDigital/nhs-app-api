package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordEncountersModule : HybridPageObject() {

    private val encountersLinkText = "Encounters"
    private val encounters by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(encountersLinkText)

    val link = encounters.link(encountersLinkText)

}