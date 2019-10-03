package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordImmunisationsModule : HybridPageObject() {

    private val immunisationsLinkText = "Immunisations"
    private val immunisations by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(immunisationsLinkText)

    val link = immunisations.link(immunisationsLinkText)

}