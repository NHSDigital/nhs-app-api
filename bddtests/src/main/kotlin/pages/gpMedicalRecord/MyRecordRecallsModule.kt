package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordRecallsModule : HybridPageObject() {

    private val recallsLinkText = "Recalls"
    private val recalls by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(recallsLinkText)

    val link = recalls.link(recallsLinkText)

}