package pages.gpMedicalRecord

import pages.HybridPageObject
import pages.sharedElements.LinksElement
import pages.sharedElements.LinksContent

open class MyRecordReferralsModule : HybridPageObject() {

    private val referralsLinkText = "Referrals"
    private val referrals by lazy {LinksElement(this, content)}
    private val content = LinksContent(
            linkBlockTitle = "",
            containerXPath = "//div[@data-purpose='medical-record-menu']")
            .addLink(referralsLinkText)

    val link = referrals.link(referralsLinkText)

}